using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovment : MonoBehaviour
{
    // Public variables can be adjusted in the Unity Inspector
    // SerializeField makes private variables visible in the Inspector, but keeps them private to the script
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;  // how high the player jumps
    [SerializeField] private SpriteRenderer spriteRenderer; // to flip the sprite based on movement direction
    [SerializeField] private Animator animator; // for handling animations

    // Input Actions
    [SerializeField] InputAction moveAction = new InputAction(type: InputActionType.Value, expectedControlType: nameof(Vector2));
    [SerializeField] InputAction jumpAction = new InputAction(type: InputActionType.Button);

    private Vector2 movement;
    private Rigidbody2D rb;  // for jumping
    private bool isGrounded = false; // to check if the player is on the ground, so they can't double jump
    private HashSet<Collider2D> groundContacts = new HashSet<Collider2D>(); // Track all ground contacts

    private Vector2 screenBounds;
    private float playerHalfWidth; // half the width of the player sprite for screen bounds calculation
    private float XPosLastFrame;

    //  Half height of the player sprite for ground check calculations 
    private float groundCheckUp = 0.5f;
    private float groundCheckDown = 0.7f;
    private float groundCheck = 0.1f;

    void OnValidate()
    {
        // Provide default bindings for the input actions
        if (moveAction.bindings.Count == 0)
        {
            moveAction.AddCompositeBinding("2DVector")
                .With("Down", "<Keyboard>/s")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/a")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/d")
                .With("Right", "<Keyboard>/rightArrow");
        }

        if (jumpAction.bindings.Count == 0)
        {
            jumpAction.AddBinding("<Keyboard>/space");
            jumpAction.AddBinding("<Keyboard>/upArrow");
            jumpAction.AddBinding("<Keyboard>/w");
        }
    }

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Get screen bounds so the player doesn't go off screen
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        playerHalfWidth = spriteRenderer.bounds.extents.x;

        // Print the player half width for debugging
        Debug.Log("Player Half Width: " + playerHalfWidth);

        spriteRenderer.flipX = false; // Initially facing right
        XPosLastFrame = transform.position.x;  // For tracking movement direction
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovment();
        ClampedMovment();
        FlipCharacterX();
        Jump();
        UpdateJumpAnimation();
    }

    private void HandleMovment()
    {
        // This function is currently not used but can be implemented for more complex movement handling
        // Get horizontal input
        // GetAxisRaw returns -1, 0 or 1 based on the input
        // "Horizontal" is the default input axis for left/right movement
        // Good with controller and keyboard

        //Debug.Log("Update called");
        //float inputX = Input.GetAxisRaw("Horizontal");

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        float inputX = moveInput.x;
        movement.x = inputX * speed * Time.deltaTime;
        transform.Translate(movement);

        if (inputX != 0)
        {
            animator.SetBool("isRunning", true);
            Debug.Log("Running animation triggered");
        }
        else
        {
            animator.SetBool("isRunning", false);
            Debug.Log("Idle animation triggered");
        }
    }

    private void ClampedMovment()
    {
        // Keep player within screen bounds
        // The 0.5f is half the width of the player sprite, assuming it's 1 unit wide, so it won't go off screen
        float clampedX = Mathf.Clamp(transform.position.x, -screenBounds.x + playerHalfWidth, screenBounds.x - playerHalfWidth);

        // Apply clamped position - like swap function
        Vector2 pos = transform.position; // Get current position of the player
        pos.x = clampedX; // Update x position
        transform.position = pos; // Apply updated position
    }

    private void FlipCharacterX()
    {
        //float inputX = Input.GetAxisRaw("Horizontal");

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        float inputX = moveInput.x;

        if (inputX > 0 && (transform.position.x > XPosLastFrame))
        {
            Debug.Log("Moving Right");
            spriteRenderer.flipX = false; // Facing right, no need to flip
        }
        else if (inputX < 0 && (transform.position.x < XPosLastFrame))
        {
            Debug.Log("Moving Left");
            spriteRenderer.flipX = true; // Facing left, flip the sprite
        }

        XPosLastFrame = transform.position.x; // Update last frame position
    }

    private void Jump()
    {
        // Jumping - check if space is pressed
        if (isGrounded && jumpAction.WasPressedThisFrame())
        {
            Debug.Log("Jump!");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            // Set to false immediately to prevent multiple jumps
            isGrounded = false;
        }
    }

    // Update jump animation based on vertical velocity
    private void UpdateJumpAnimation()
    {
        // This ensures animation triggers immediately when jumping
        if (!isGrounded || rb.linearVelocity.y > 0.1f)
        {
            animator.SetBool("isJumping", true);
            Debug.Log("Jumping animation triggered");
        }
        else
        {
            animator.SetBool("isJumping", false);
            Debug.Log("Not jumping");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Don't ground if moving upward significantly
            if (rb.linearVelocity.y > groundCheckUp)
            {
                Debug.Log("Hit platform from below, ignoring");
                return;
            }

            // Check if we're landing on TOP of the object
            // by examining the collision normal (direction of impact)
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // If the normal points upward (y > 0.7), we're landing on top
                if (contact.normal.y > groundCheckDown)
                {
                    // Only set grounded if NOT moving upward
                    if (rb.linearVelocity.y <= groundCheck)
                    {
                        groundContacts.Add(collision.collider);
                        isGrounded = true;
                        Debug.Log("Landed on ground from TOP");
                        break;
                    }
                }
                else
                {
                    Debug.Log($"Collision from side/bottom (normal.y: {contact.normal.y})");
                }
            }
        }

        // If touching the clone the level start again
        if (collision.gameObject.CompareTag("Clone"))
        {
            if (DeathTracker.Instance != null)
            {
                DeathTracker.Instance.RecordDeath();
            }

            Debug.Log("Touched clone! Restarting level...");

            // Freeze everything immediately
            Time.timeScale = 0f;

            // Show death message with reason
            if (DeathMessage.Instance != null)
            {
                DeathMessage.Instance.ShowDeathMessage("Hit by your clone!");
            }
            else
            {
                // Fallback if no death message system exists
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Remove this contact from our set
            groundContacts.Remove(collision.collider);

            // Only set to false if we have NO ground contacts left
            if (groundContacts.Count == 0)
            {
                isGrounded = false;
                Debug.Log("Left ground completely");
            }
        }
    }
}