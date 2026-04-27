using UnityEngine;

public class RustNPCDialogue : MonoBehaviour, IInteractable
{
    [Header("Dialogue Source")]
    [SerializeField] private DialogueData dialogueData;
    
    [SerializeField] private TextAsset preGameDialogueJson;
    [SerializeField] private TextAsset postGameDialogueJson;

    // [SerializeField] private TextAsset DialogueJson;
    [SerializeField] private string dialogueJsonResourcesPath;
    [TextArea(4, 16)]
    [SerializeField] private string dialogueJsonText;

    [Header("Dialogue")]

    [Header("UI")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private Animator anim;
    
    private DialogueData runtimeJsonDialogue;

    private void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        if (dialogueData == null && preGameDialogueJson == null && postGameDialogueJson == null)
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

        DialogueData resolvedDialogue;

        if (RoboticsGameManager.CompleteChallenge) resolvedDialogue = GetResolvedDialogue(postGameDialogueJson);
        else resolvedDialogue = GetResolvedDialogue(preGameDialogueJson);

        // DialogueData resolvedDialogue = GetResolvedDialogue();
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
        if (anim != null)
        {
            // Check if the Animator has the 'isGreeting' parameter to avoid errors
            // if (HasParameter("isGreeting", anim))
            // {
                anim.SetBool("isGreeting", true);
            // }
        }
    }

    public void HidePrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
            anim.SetBool("isGreeting", false);
        }
    }

    private DialogueData GetResolvedDialogue(TextAsset dialogueJson)
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
            if (!string.IsNullOrWhiteSpace(dialogueJsonResourcesPath))
            {
                TextAsset resourceJson = Resources.Load<TextAsset>(dialogueJsonResourcesPath);
                if (resourceJson != null)
                {
                    runtimeJsonDialogue = DialogueJsonLoader.LoadFromTextAsset(resourceJson);
                    return runtimeJsonDialogue;
                }

                Debug.LogWarning($"{name}: Could not load TextAsset at Resources path '{dialogueJsonResourcesPath}'.");
            }

            if (!string.IsNullOrWhiteSpace(dialogueJsonText))
            {
                runtimeJsonDialogue = DialogueJsonLoader.LoadFromJson(dialogueJsonText, $"{name} inline JSON");
                return runtimeJsonDialogue;
            }

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