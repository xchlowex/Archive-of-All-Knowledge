using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private const string EvaluateHumanityNodeToken = "__EVALUATE_HUMANITY__";
    private static readonly HashSet<string> consumedDialogueIds = new HashSet<string>();
    private static readonly HashSet<int> consumedDialogueInstances = new HashSet<int>();

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button choiceButtonPrefab;
    
    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.05f;

    [Header("Ending Evaluation")]
    [SerializeField] private int humanEndingThreshold = 0;
    [SerializeField] private string humanEndingNodeId = "human";
    [SerializeField] private string machineEndingNodeId = "machine";
    
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
        if (dialogue == null || dialogue.nodes == null || dialogue.nodes.Count == 0)
        {
            Debug.LogWarning("StartDialogue called with empty dialogue data.");
            return;
        }

        if (dialogue.consumeAfterFirstUse && IsDialogueConsumed(dialogue))
        {
            Debug.Log($"Dialogue '{GetDialogueLabel(dialogue)}' has already been consumed.");
            return;
        }

        DialogueNode startNode = dialogue.nodes.Find(node => node.nodeId == dialogue.startNodeId);
        if (startNode == null)
        {
            Debug.LogWarning($"StartDialogue could not find start node '{dialogue.startNodeId}'.");
            return;
        }

        if (startNode.lines == null || startNode.lines.Count == 0)
        {
            Debug.LogWarning($"StartDialogue found start node '{dialogue.startNodeId}' but it has no lines.");
                EndDialogue(false);
                return;
        }

        currentDialogue = dialogue;
        currentNode = startNode;
        currentLineIndex = 0;

        if (dialogue.consumeAfterFirstUse)
        {
            MarkDialogueConsumed(dialogue);
        }
        
        dialoguePanel.SetActive(true);
        DisplayCurrentLine();
    }

    private static bool IsDialogueConsumed(DialogueData dialogue)
    {
        if (!dialogue.consumeAfterFirstUse)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(dialogue.dialogueId))
        {
            return consumedDialogueIds.Contains(dialogue.dialogueId);
        }

        return consumedDialogueInstances.Contains(dialogue.GetInstanceID());
    }

    private static void MarkDialogueConsumed(DialogueData dialogue)
    {
        if (!dialogue.consumeAfterFirstUse)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(dialogue.dialogueId))
        {
            consumedDialogueIds.Add(dialogue.dialogueId);
            return;
        }

        consumedDialogueInstances.Add(dialogue.GetInstanceID());
    }

    private static string GetDialogueLabel(DialogueData dialogue)
    {
        if (!string.IsNullOrWhiteSpace(dialogue.dialogueId))
        {
            return dialogue.dialogueId;
        }

        return dialogue.name;
    }
    
    private void DisplayCurrentLine()
    {
        if (currentNode == null || currentNode.lines == null || currentLineIndex >= currentNode.lines.Count)
        {
            EndDialogue(false);
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

        bool hasLayoutGroup = choicePanel.GetComponent<LayoutGroup>() != null;
        int index = 0;
        
        foreach (var choice in choices)
        {
            Button choiceButton = Instantiate(choiceButtonPrefab, choicePanel.transform);
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            if (!hasLayoutGroup)
            {
                RectTransform buttonRect = choiceButton.GetComponent<RectTransform>();
                if (buttonRect != null)
                {
                    float buttonHeight = buttonRect.sizeDelta.y > 0f ? buttonRect.sizeDelta.y : 40f;
                    float spacing = buttonHeight + 10f;

                    buttonRect.anchorMin = new Vector2(0.5f, 1f);
                    buttonRect.anchorMax = new Vector2(0.5f, 1f);
                    buttonRect.pivot = new Vector2(0.5f, 1f);
                    buttonRect.anchoredPosition = new Vector2(0f, -index * spacing);
                }
            }
            
            // Store choice data in button
            var choiceData = choice;
            choiceButton.onClick.AddListener(() => OnChoiceSelected(choiceData));
            index++;
        }
    }
    
    private void OnChoiceSelected(DialogueChoice choice)
    {
        // Apply humanity modifier
        if (choice.humanityModifier != 0)
        {
            GameManager.Instance?.ModifyHumanity(choice.humanityModifier);
        }

        // Optional quest completion hook from dialogue data.
        if (choice.marksIslandComplete && choice.completedIslandIndex >= 0)
        {
            GameManager.Instance?.CompleteStar(choice.completedIslandIndex);
        }
        
        // Clear choices
        foreach (Transform child in choicePanel.transform)
            Destroy(child.gameObject);
        choicePanel.SetActive(false);

        string resolvedNextNodeId = ResolveNextNodeId(choice.nextNodeId);

        // Move to next node if specified
        if (!string.IsNullOrEmpty(resolvedNextNodeId))
        {
            currentNode = currentDialogue.nodes.Find(node => node.nodeId == resolvedNextNodeId);
            if (currentNode == null)
            {
                Debug.LogWarning($"Dialogue node '{resolvedNextNodeId}' was not found.");
                EndDialogue(false);
                return;
            }

            currentLineIndex = 0;
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue(false);
        }
    }

    private string ResolveNextNodeId(string nextNodeId)
    {
        if (nextNodeId != EvaluateHumanityNodeToken)
        {
            return nextNodeId;
        }

        int humanityScore = GameManager.Instance != null ? GameManager.Instance.GetHumanityScore() : 0;
        return humanityScore >= humanEndingThreshold ? humanEndingNodeId : machineEndingNodeId;
    }
    
    public void ContinueDialogue()
    {
        if (currentNode == null || currentNode.lines == null)
        {
            EndDialogue(false);
            return;
        }

        // Do not advance lines while player is choosing an option.
        if (choicePanel != null && choicePanel.activeSelf)
        {
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
            EndDialogue(true);
        }
        else
        {
            DisplayCurrentLine();
        }
    }
    
    private void EndDialogue(bool completedNaturally)
    {
        DialogueData completedDialogue = currentDialogue;

        if (completedNaturally && completedDialogue != null && completedDialogue.completesIslandOnEnd && completedDialogue.completedIslandIndex >= 0)
        {
            GameManager.Instance?.CompleteStar(completedDialogue.completedIslandIndex);
        }

        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        currentDialogue = null;
        currentNode = null;

        if (completedNaturally && completedDialogue != null && completedDialogue.endsGameOnEnd)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("Dialogue ended the game.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    private void Update()
    {
        // Quick continue with mouse click or spacebar, but never while choices are on screen.
        bool choicesVisible = choicePanel != null && choicePanel.activeSelf;
        if (dialoguePanel.activeSelf && !choicesVisible && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            ContinueDialogue();
        }
    }
}