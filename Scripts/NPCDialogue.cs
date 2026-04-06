using UnityEngine;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [Header("Dialogue Source")]
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private TextAsset dialogueJson;

    [Header("UI")]
    [SerializeField] private GameObject interactionPrompt;

    private DialogueData runtimeJsonDialogue;

    private void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        if (dialogueData == null && dialogueJson == null)
        {
            Debug.LogWarning($"{name}: NPCDialogue has no DialogueData or DialogueJson assigned.");
        }
    }

    public void Interact()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning($"{name}: DialogueManager.Instance is missing in scene.");
            return;
        }

        DialogueData resolvedDialogue = GetResolvedDialogue();
        if (resolvedDialogue != null)
        {
            DialogueManager.Instance.StartDialogue(resolvedDialogue);
        }
        else
        {
            Debug.LogWarning($"{name}: Could not resolve dialogue source.");
        }
    }

    public void ShowPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }

    public void HidePrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private DialogueData GetResolvedDialogue()
    {
        if (dialogueData != null)
        {
            return dialogueData;
        }

        if (runtimeJsonDialogue != null)
        {
            return runtimeJsonDialogue;
        }

        if (dialogueJson == null)
        {
            return null;
        }

        runtimeJsonDialogue = DialogueJsonLoader.LoadFromTextAsset(dialogueJson);
        return runtimeJsonDialogue;
    }

    private void OnDestroy()
    {
        if (runtimeJsonDialogue != null)
        {
            Destroy(runtimeJsonDialogue);
            runtimeJsonDialogue = null;
        }
    }
}