using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int IsRunningX = Animator.StringToHash("isRunningX");
    private static readonly int IsRunningLeft = Animator.StringToHash("isRunningLeft");
    private static readonly int IsRunningFront = Animator.StringToHash("isRunningFront");
    private static readonly int IsRunningBack = Animator.StringToHash("isRunningBack");

    [Header("Component References")]
    [SerializeField] private Animator animator;
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 moveInput;

    // Start is called before the first frame update
    void Start()
    {
        // Cache the Rigidbody once at the start to save performance
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("PlayerMovement requires a Rigidbody2D on the same GameObject.");
            enabled = false;
            return;
        }

        // Keep top-down character stable in Play Mode.
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Optional: Auto-fill animator if you forgot to drag it in the inspector
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            Debug.LogWarning("PlayerMovement could not find an Animator component.");
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = GetComponentInChildren<SpriteRenderer>();
        }

        if (sr == null)
        {
            Debug.LogWarning("No SpriteRenderer found on Player. The character may be invisible in Play Mode.");
        }
        else if (sr.sprite == null)
        {
            Debug.LogWarning("SpriteRenderer has no sprite assigned at startup. Check idle animation clip and default sprite.");
        }

        if (Mathf.Approximately(transform.localScale.x, 0f) || Mathf.Approximately(transform.localScale.y, 0f))
        {
            Debug.LogWarning("Player scale is zero on at least one axis, making it invisible.");
        }
    }

    // Update is called once per frame (Better for Input)
    void Update()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            moveInput = Vector2.zero;
            HandleAnimations();
            return;
        }

        // Capture input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Run animation logic
        HandleAnimations();
    }

    // FixedUpdate is called at a constant rate (Best for Physics/Rigidbody)
    void FixedUpdate()
    {
        // Apply velocity to the Rigidbody
        rb.velocity = moveInput.normalized * moveSpeed;
    }

    private void HandleAnimations()
    {
        if (animator == null) return;

        // 1. Prioritize Horizontal (X)
        if (moveInput.x > 0)
        {
            animator.SetBool(IsRunningX, true);
            animator.SetBool(IsRunningLeft, false);
            animator.SetBool(IsRunningFront, false);
            animator.SetBool(IsRunningBack, false);
        }
        else if (moveInput.x < 0)
        {
            animator.SetBool(IsRunningX, false);
            animator.SetBool(IsRunningLeft, true);
            animator.SetBool(IsRunningFront, false);
            animator.SetBool(IsRunningBack, false);
        }
        // 2. Vertical (Y)
        else if (moveInput.y != 0)
        {
            animator.SetBool(IsRunningX, false);
            animator.SetBool(IsRunningLeft, false);
            animator.SetBool(IsRunningBack, moveInput.y > 0);
            animator.SetBool(IsRunningFront, moveInput.y < 0);
        }
        // 3. Idle
        else
        {
            animator.SetBool(IsRunningX, false);
            animator.SetBool(IsRunningLeft, false);
            animator.SetBool(IsRunningFront, false);
            animator.SetBool(IsRunningBack, false);
        }
    }
}