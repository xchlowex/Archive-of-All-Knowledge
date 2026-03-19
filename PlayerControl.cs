using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactionRange = 1.5f;
    
    [Header("References")]
    [SerializeField] private LayerMask interactableLayer;
    
    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    // Movement variables
    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.down; // Default facing down
    
    // Interaction
    private IInteractable currentInteractable;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (rb == null)
            Debug.LogError("PlayerController: Rigidbody2D component missing!");
    }
    
    private void Update()
    {
        // Handle animation
        UpdateAnimation();
        
        // Check for nearby interactables
        CheckForInteractables();
    }
    
    private void FixedUpdate()
    {
        // Move the player (physics should be in FixedUpdate)
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
    
    // Called by the Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        
        // Store last non-zero direction for facing direction
        if (moveInput != Vector2.zero)
        {
            lastDirection = moveInput;
        }
    }
    
    // Called by the Input System
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }
    
    private void UpdateAnimation()
    {
        if (animator != null)
        {
            // Set animation parameters
            animator.SetFloat("MoveX", moveInput.x);
            animator.SetFloat("MoveY", moveInput.y);
            animator.SetFloat("LastMoveX", lastDirection.x);
            animator.SetFloat("LastMoveY", lastDirection.y);
            animator.SetBool("IsMoving", moveInput != Vector2.zero);
        }
        
        // Flip sprite for left/right (if not using animation)
        if (spriteRenderer != null && moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
        }
    }
    
    private void CheckForInteractables()
    {
        // Simple overlap circle to find nearby interactables
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);
        
        IInteractable closestInteractable = null;
        float closestDistance = float.MaxValue;
        
        foreach (var hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }
        
        // Update current interactable
        if (closestInteractable != currentInteractable)
        {
            // Hide previous prompt
            if (currentInteractable != null)
                currentInteractable.HidePrompt();
            
            currentInteractable = closestInteractable;
            
            // Show new prompt
            if (currentInteractable != null)
                currentInteractable.ShowPrompt();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Visualize interaction range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}

/*to be implmented in unity - Scripts/Interfaces
public interface IInteractable
{
    void Interact();
    void ShowPrompt();
    void HidePrompt();
}
*/