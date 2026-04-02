using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;

    private Vector2 moveInput;
    private readonly HashSet<IInteractable> interactablesInRange = new HashSet<IInteractable>();
    private IInteractable currentInteractable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("PlayerControl requires a Rigidbody2D component.");
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        ReadMovementInput();
        HandleAnimations();

        if (Input.GetKeyDown(interactKey) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        rb.velocity = moveInput.normalized * moveSpeed;
    }

    private void ReadMovementInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
    }

    private void HandleAnimations()
    {
        if (animator == null) return;

        // 1. Prioritize Horizontal (X)
        if (moveInput.x != 0)
        {
            animator.SetBool("isRunningX", true);
            animator.SetBool("isRunningFront", false);
            animator.SetBool("isRunningBack", false);

            // Flip sprite: 1 for right, -1 for left
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x), 1, 1);
        }
        // 2. Vertical (Y)
        else if (moveInput.y != 0)
        {
            animator.SetBool("isRunningX", false);

            // If moving UP (Y > 0), show BACK. If moving DOWN (Y < 0), show FRONT.
            animator.SetBool("isRunningBack", moveInput.y > 0);
            animator.SetBool("isRunningFront", moveInput.y < 0);
        }
        // 3. Idle
        else
        {
            animator.SetBool("isRunningX", false);
            animator.SetBool("isRunningFront", false);
            animator.SetBool("isRunningBack", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable == null)
        {
            return;
        }

        if (interactablesInRange.Add(interactable))
        {
            interactable.ShowPrompt();
            RefreshCurrentInteractable();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable == null)
        {
            return;
        }

        if (interactablesInRange.Remove(interactable))
        {
            interactable.HidePrompt();

            if (currentInteractable == interactable)
            {
                currentInteractable = null;
                RefreshCurrentInteractable();
            }
        }
    }

    private void RefreshCurrentInteractable()
    {
        currentInteractable = null;

        foreach (IInteractable interactable in interactablesInRange)
        {
            currentInteractable = interactable;
            break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}

public interface IInteractable
{
    void Interact();
    void ShowPrompt();
    void HidePrompt();
}