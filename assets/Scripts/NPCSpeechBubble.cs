using UnityEngine;

public class NPCBase : MonoBehaviour, IInteractable
{
    [Header("UI & Visuals")]
    [SerializeField] private GameObject interactionPrompt; // The Speech Bubble/E
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Animation Parameters")]
    [SerializeField] private string greetTrigger = "greet";
    [SerializeField] private string talkBool = "isTalking";

    public virtual void ShowPrompt()
    {
        Debug.Log("Trigger received by " + gameObject.name);
        if (interactionPrompt != null) interactionPrompt.SetActive(true);
        FacePlayer();
        
        if (animator != null) animator.SetTrigger(greetTrigger);
    }

    public virtual void HidePrompt()
    {
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
        SetTalking(false); // Stop talking if player walks away
    }

    public virtual void Interact()
    {
        Debug.Log($"{gameObject.name} is interacting!");
        // Your Global Dialogue System call goes here
    }

    public void SetTalking(bool isTalking)
    {
        if (animator != null) animator.SetBool(talkBool, isTalking);
    }

    private void FacePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && spriteRenderer != null)
        {
            // Flip sprite based on player position
            spriteRenderer.flipX = player.transform.position.x < transform.position.x;
        }
    }
}