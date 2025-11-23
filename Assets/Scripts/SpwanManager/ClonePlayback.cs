using UnityEngine;
using System.Collections.Generic;

public class ClonePlayback : MonoBehaviour
{
    private List<Vector2> positions;
    private List<Quaternion> rotations;
    private List<Vector2> scales;
    private List<bool> flipXStates;
    private List<bool> isRunningStates;
    private int currentFrame = 0;
    private bool isPlaying = false;

    //[SerializeField] private RecorderManager recorderManager;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private float frameNumber = 50f; // If we want to control how often we log playback info

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

        //if(recorderManager == null)
        //{
        //    Debug.LogError("[CloneSpawner] RecorderManager not assigned!");
        //}
    }

    public void StartPlayback(List<Vector2> pos, List<Quaternion> rot, List<Vector2> scl, List<bool> flipX, List<bool> running)
    {
        positions = pos;
        rotations = rot;
        scales = scl;
        flipXStates = flipX;
        isRunningStates = running;
        currentFrame = 0; // initialize current frame to zero because starting form begigning of the frame
        isPlaying = true;
        Debug.Log($"[ClonePlayback] Started playback with {positions.Count} frames");
    }

    void Update()
    {
        if (isPlaying && currentFrame < positions.Count)
        {
            transform.position = positions[currentFrame];
            transform.rotation = rotations[currentFrame];
            transform.localScale = scales[currentFrame];
            spriteRenderer.flipX = flipXStates[currentFrame];
            animator.SetBool("isRunning", isRunningStates[currentFrame]);
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
        }
    }
}