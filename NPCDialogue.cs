using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [Header("Dialogue")]
    [SerializeField] private DialogueData dialogueData;
    
    [Header("UI")]
    [SerializeField] private GameObject interactionPrompt;
    
    private bool playerInRange;
    
    private void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    public void Interact()
    {
        if (dialogueData != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueData);
        }
    }
    
    public void ShowPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
            // Optional: animate prompt
        }
    }
    
    public void HidePrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }
    
    // Optional: Trigger-based approach instead of PlayerController's overlap check
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowPrompt();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HidePrompt();
        }
    }
}