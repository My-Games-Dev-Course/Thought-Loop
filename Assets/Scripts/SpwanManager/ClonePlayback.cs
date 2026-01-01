using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Rendering;

public class ClonePlayback : MonoBehaviour
{
    private List<Vector2> positions;
    private List<Quaternion> rotations;
    private List<Vector2> scales;
    private List<bool> flipXStates;
    private List<bool> isRunningStates;
    private List<bool> isJumpingStates;
    private int currentFrame = 0;
    private bool isPlaying = false;

    //[SerializeField] private RecorderManager recorderManager;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private float frameNumber = 50f; // If we want to control how often we log playback info

    [SerializeField] private Rigidbody2D rb;  // NEW: Reference to Rigidbody2D


    [Header("Fall Behavior (Optional)")]
    [Tooltip("If true, clone will fall to ground after playback ends. If false, stays in air.")]
    [SerializeField] private bool fallAfterPlayback = true;

    [Tooltip("Gravity scale when falling (default: 2 to match player)")]
    [SerializeField] private float fallGravityScale = 2f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }


        // Get Rigidbody2D
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        // Validation
        if (rb == null)
        {
            Debug.LogWarning("[ClonePlayback] No Rigidbody2D found! Clone won't fall after playback.");
            fallAfterPlayback = false;
        }
    }

    public Rigidbody2D GetRb()
    {
        return rb;
    }

    public void StartPlayback(List<Vector2> pos, List<Quaternion> rot, List<Vector2> scl, List<bool> flipX, List<bool> running, List<bool> jumping, Rigidbody2D rb)
    {
        positions = pos;
        rotations = rot;
        scales = scl;
        flipXStates = flipX;
        isRunningStates = running;
        isJumpingStates = jumping;
        currentFrame = 0; // initialize current frame to zero because starting form begigning of the frame
        isPlaying = true;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;  // Modern Unity API
            rb.linearVelocity = Vector2.zero;  // Clear any velocity
            Debug.Log("[ClonePlayback] Set to KINEMATIC for playback (position controlled by recording)");
        }
        Debug.Log($"[ClonePlayback] Started playback with {positions.Count} frames");
    }

    void Update()
    {
        // Stop playback when the game is paused(victory popup up)
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (isPlaying && currentFrame < positions.Count)
        {
            transform.position = positions[currentFrame];
            transform.rotation = rotations[currentFrame];
            transform.localScale = scales[currentFrame];
            spriteRenderer.flipX = flipXStates[currentFrame];
            animator.SetBool("isRunning", isRunningStates[currentFrame]);
            //animator.SetBool("isJumping", isJumpingStates[currentFrame]);
            // Set jumping animation state
            if (currentFrame < isJumpingStates.Count)
            {
                animator.SetBool("isJumping", isJumpingStates[currentFrame]);
            }
            currentFrame++;

            if (currentFrame % frameNumber == 0)
            {
                Debug.Log($"[ClonePlayback] Playing frame {currentFrame}/{positions.Count} | Pos: {transform.position}");
            }
        }
        else if (isPlaying)
        {
            isPlaying = false;
            Debug.Log($"[ClonePlayback] Playback finished at frame {currentFrame}");
            OnPlaybackComplete();
        }
    }


    // Called when playback ends. Enables physics so clone falls naturally.
    void OnPlaybackComplete()
    {
        if (!fallAfterPlayback || rb == null)
        {
            Debug.Log("[ClonePlayback] Fall disabled or no Rigidbody - clone stays in position");
            return;
        }

        // Enable physics using modern Unity API
        rb.bodyType = RigidbodyType2D.Dynamic;  // Modern Unity API
        rb.gravityScale = fallGravityScale;

        // Set clone to IDLE animation (stop running and jumping)
        if (animator != null)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isJumping", false);
            Debug.Log("[ClonePlayback] Animation set to IDLE");
        }

        Debug.Log($"[ClonePlayback] Enabled PHYSICS - clone will now fall to ground (gravity: {fallGravityScale})");
    }

    // Check if playback is still active
    public bool IsPlaying()
    {
        return isPlaying;
    }

    // Get current playback frame
    public int GetCurrentFrame()
    {
        return currentFrame;
    }

    // Get total number of recorded frames
    public int GetTotalFrames()
    {
        return positions?.Count ?? 0;
    }

    // Check if clone is currently falling (physics enabled)
    public bool IsFalling()
    {
        return rb != null && rb.bodyType == RigidbodyType2D.Dynamic && !isPlaying;
    }
}
