// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerMovement : MonoBehaviour
// {
//     [SerializeField] private Animator animator;
//     // public GameObject bullet;
//     private Rigidbody2D rb;
//     private Vector2 v;

//     // Start is called before the first frame update
//     // void Start()
//     // {
        
//     // }

//     // // Update is called once per frame
//     // void Update()
//     // {
//     //     rb = GetComponent<Rigidbody2D>();
//     //     v= rb.velocity;
//     //     v.x = Input.GetAxis("Horizontal") * 10;
//     //     v.y = Input.GetAxis("Vertical") * 10;
//     //     rb.velocity = v;
//     //     /* if (Input.GetKeyDown("space"))
//     //     {
//     //         Instantiate(bullet, transform.position, Quaternion.identity);
//     //     } */
//     // }
    
//     void Start()
//     {
//         rb = GetComponent<Rigidbody2D>();
//     }

//     void Update()
//     {
//         // Now just use rb here without calling GetComponent again
//     }
//     private void HandleMovement()
//     {
//         if (v.x != 0){
//             animator.SetBool("isRunningFront", false)
//             animator.SetBool("isRunningBack", false)
//             animator.SetBool("isRunningX", true)

//         }
//         else if (v.y >0) {
//             animator.SetBool("isRunningX", false)
//             animator.SetBool("isRunningBack", false)
//             animator.SetBool("isRunningFront", true)
//         }
//         else if (v.y <0) {
//             animator.SetBool("isRunningFront", false)
//             animator.SetBool("isRunningX", false)
//             animator.SetBool("isRunningBack", true)
//         }
//         else if (v.y = 0) {
//             animator.SetBool("isRunningX", false)
//             animator.SetBool("isRunningFront", false)
//             animator.SetBool("isRunningBack", false)
//         }
//     }
// }   








using UnityEngine;

// IMPORTANT: Ensure the filename in Unity is "PlayerMovement.cs"
public class PlayerMovement : MonoBehaviour
{
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