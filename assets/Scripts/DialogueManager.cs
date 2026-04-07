using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button choiceButtonPrefab;
    
    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.05f;
    
    // State
    private DialogueData currentDialogue;
    private DialogueNode currentNode;
    private int currentLineIndex;
    private bool isTyping;
    private Coroutine typingCoroutine;
    
    // Singleton for easy access
    public static DialogueManager Instance { get; private set; }
    public bool IsDialogueActive => dialoguePanel != null && dialoguePanel.activeSelf;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
    }
    
    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentNode = dialogue.nodes.Find(node => node.nodeId == dialogue.startNodeId);
        currentLineIndex = 0;
        
        dialoguePanel.SetActive(true);
        DisplayCurrentLine();
    }
    
    private void DisplayCurrentLine()
    {
        if (currentNode == null || currentLineIndex >= currentNode.lines.Count)
        {
            EndDialogue();
            return;
        }
        
        DialogueLine currentLine = currentNode.lines[currentLineIndex];
        
        // Set speaker
        speakerText.text = currentLine.speakerName;
        
        // Clear any existing choices
        foreach (Transform child in choicePanel.transform)
            Destroy(child.gameObject);
        
        choicePanel.SetActive(false);
        
        // Start typing effect
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(currentLine.text));
        
        // If this line has choices, show them after text finishes
        if (currentLine.hasChoice)
        {
            StartCoroutine(WaitAndShowChoices(currentLine.choices));
        }
    }
    
    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char c in text.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        
        isTyping = false;
    }
    
    private IEnumerator WaitAndShowChoices(List<DialogueChoice> choices)
    {
        // Wait until typing is done
        while (isTyping)
            yield return null;
            
        // Show choices
        ShowChoices(choices);
    }
    
    private void ShowChoices(List<DialogueChoice> choices)
    {
        choicePanel.SetActive(true);
        
        foreach (var choice in choices)
        {
            Button choiceButton = Instantiate(choiceButtonPrefab, choicePanel.transform);
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
            
            // Store choice data in button
            var choiceData = choice;
            choiceButton.onClick.AddListener(() => OnChoiceSelected(choiceData));
        }
    }
    
    private void OnChoiceSelected(DialogueChoice choice)
    {
        // Apply humanity modifier
        if (choice.humanityModifier != 0)
        {
            GameManager.Instance?.ModifyHumanity(choice.humanityModifier);
        }
        
        // Clear choices
        foreach (Transform child in choicePanel.transform)
            Destroy(child.gameObject);
        choicePanel.SetActive(false);
        
        // Move to next node if specified
        if (!string.IsNullOrEmpty(choice.nextNodeId))
        {
            currentNode = currentDialogue.nodes.Find(node => node.nodeId == choice.nextNodeId);
            currentLineIndex = 0;
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }
    
    public void ContinueDialogue()
    {
        if (currentNode == null || currentNode.lines == null)
        {
            EndDialogue();
            return;
        }

        if (isTyping)
        {
            // Skip typing
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentNode.lines[currentLineIndex].text;
            isTyping = false;
            return;
        }
        
        // Move to next line
        currentLineIndex++;
        
        if (currentLineIndex >= currentNode.lines.Count)
        {
            // End of node
            EndDialogue();
        }
        else
        {
            DisplayCurrentLine();
        }
    }
    
    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        currentDialogue = null;
        currentNode = null;
    }
    
    private void Update()
    {
        // Quick continue with mouse click or spacebar
        if (dialoguePanel.activeSelf && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            ContinueDialogue();
        }
    }
}