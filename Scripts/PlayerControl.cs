using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private static readonly int IsRunningX = Animator.StringToHash("isRunningX");
    private static readonly int IsRunningLeft = Animator.StringToHash("isRunningLeft");
    private static readonly int IsRunningFront = Animator.StringToHash("isRunningFront");
    private static readonly int IsRunningBack = Animator.StringToHash("isRunningBack");

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private const float InputDeadzone = 0.01f;

    private readonly HashSet<IInteractable> interactablesInRange = new HashSet<IInteractable>();
    private IInteractable currentInteractable;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("PlayerControl requires a Rigidbody2D component.");
            enabled = false;
            return;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogWarning("PlayerControl could not find an Animator on this object or its children.");
        }
    }

    private void Update()
    {
        // Freeze player controls while a dialogue is open.
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            moveInput = Vector2.zero;
            HandleAnimations();
            return;
        }

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        HandleAnimations();
        RefreshCurrentInteractable();

        if (Input.GetKeyDown(interactKey) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = moveInput.normalized * moveSpeed;
    }

    private void HandleAnimations()
    {
        if (animator == null)
        {
            return;
        }

        bool runningX = false;
        bool runningLeft = false;
        bool runningFront = false;
        bool runningBack = false;

        // Horizontal movement has priority over vertical to match the controller setup.
        if (moveInput.x < -InputDeadzone)
        {
            runningLeft = true;
        }
        else if (moveInput.x > InputDeadzone)
        {
            runningX = true;
        }
        else if (moveInput.y > InputDeadzone)
        {
            runningBack = true;
        }
        else if (moveInput.y < -InputDeadzone)
        {
            runningFront = true;
        }

        animator.SetBool(IsRunningX, runningX);
        animator.SetBool(IsRunningLeft, runningLeft);
        animator.SetBool(IsRunningFront, runningFront);
        animator.SetBool(IsRunningBack, runningBack);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = GetInteractableFromCollider(other);
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
        IInteractable interactable = GetInteractableFromCollider(other);
        if (interactable == null)
        {
            return;
        }

        if (interactablesInRange.Remove(interactable))
        {
            interactable.HidePrompt();
            RefreshCurrentInteractable();
        }
    }

    private void RefreshCurrentInteractable()
    {
        interactablesInRange.RemoveWhere(item => item == null);

        IInteractable bestCandidate = null;
        float bestDistanceSqr = float.MaxValue;

        foreach (IInteractable interactable in interactablesInRange)
        {
            if (interactable is not Component component)
            {
                continue;
            }

            float distanceSqr = (component.transform.position - transform.position).sqrMagnitude;
            if (distanceSqr < bestDistanceSqr)
            {
                bestDistanceSqr = distanceSqr;
                bestCandidate = interactable;
            }
        }

        currentInteractable = bestCandidate;
    }

    private static IInteractable GetInteractableFromCollider(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            return interactable;
        }

        return other.GetComponentInParent<IInteractable>();
    }

    private void OnDisable()
    {
        foreach (IInteractable interactable in interactablesInRange)
        {
            interactable?.HidePrompt();
        }

        interactablesInRange.Clear();
        currentInteractable = null;
    }
}

public interface IInteractable
{
    void Interact();
    void ShowPrompt();
    void HidePrompt();
}
