using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Animation")]
    [SerializeField] private string moveXParameter = "MoveX";
    [SerializeField] private string moveYParameter = "MoveY";
    [SerializeField] private string lastMoveXParameter = "LastMoveX";
    [SerializeField] private string lastMoveYParameter = "LastMoveY";
    [SerializeField] private string isMovingParameter = "IsMoving";

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.down;
    private readonly HashSet<IInteractable> interactablesInRange = new HashSet<IInteractable>();
    private IInteractable currentInteractable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
        {
            Debug.LogError("PlayerControl requires a Rigidbody2D component.");
        }
    }

    private void Update()
    {
        ReadMovementInput();
        UpdateAnimation();

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

        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void ReadMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(horizontal, vertical);
        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }

        if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput;
        }
    }

    private void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat(moveXParameter, moveInput.x);
            animator.SetFloat(moveYParameter, moveInput.y);
            animator.SetFloat(lastMoveXParameter, lastMoveDirection.x);
            animator.SetFloat(lastMoveYParameter, lastMoveDirection.y);
            animator.SetBool(isMovingParameter, moveInput != Vector2.zero);
        }

        if (spriteRenderer != null && moveInput.x != 0f)
        {
            spriteRenderer.flipX = moveInput.x < 0f;
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