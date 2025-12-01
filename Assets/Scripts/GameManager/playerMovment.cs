using System.Collections;
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

    private Vector2 screenBounds;
    private float playerHalfWidth; // half the width of the player sprite for screen bounds calculation
    private float XPosLastFrame;


    void OnValidate()
    {
        // Provide default bindings for the input actions
        if (moveAction.bindings.Count == 0)
        {
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Up", "<Keyboard>/upArrow")
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
        }
    }

    // Update jump animation based on vertical velocity
    private void UpdateJumpAnimation()
    {
        // If player is moving upward (jumping) or downward (falling) significantly
        if (!isGrounded)
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
            isGrounded = true;
            Debug.Log("Landed on ground");
        }

        // If touching the clone the level start again
        if (collision.gameObject.CompareTag("Clone"))
        {
            Debug.Log("Touched clone! Restarting level...");
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
    }


    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Left ground");
        }
    }
}
