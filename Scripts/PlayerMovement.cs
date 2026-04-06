using UnityEngine;

// IMPORTANT: Ensure the filename in Unity is "PlayerMovement.cs"
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

        // Optional: Auto-fill animator if you forgot to drag it in the inspector
        if (animator == null)
        {
            animator = GetComponent<Animator>();
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