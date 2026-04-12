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

    [SerializeField] private SpriteRenderer spriteRenderer;
    

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

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();        }
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
        
        if (moveInput.x != 0)
        {
            animator.SetBool("isRunningX", true);
            animator.SetBool("isRunningFront", false);
            animator.SetBool("isRunningBack", false);

            // Replace '2f' with whatever scale you actually want (e.g., 1.5f, 3.0f)
        // float playerSize = 1.5f; 
        // transform.localScale = new Vector3(Mathf.Sign(moveInput.x) * playerSize, playerSize, 1);
            spriteRenderer.flipX = moveInput.x < 0;
        }
        // 2. Vertical (Y) - FIXED LOGIC HERE
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
    }
}





