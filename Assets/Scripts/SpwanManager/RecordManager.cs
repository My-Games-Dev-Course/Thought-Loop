using UnityEngine;
using System.Collections.Generic;

public class RecorderManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private bool isRecording = true;
    [SerializeField] private float frameNumber = 50f;

    // Store position, rotation, scale, flipX, and animation states
    private List<Vector2> positions = new List<Vector2>();
    private List<Quaternion> rotations = new List<Quaternion>();
    private List<Vector2> scales = new List<Vector2>();
    private List<bool> flipXStates = new List<bool>();
    private List<bool> isRunningStates = new List<bool>();
    private List<bool> isJumpingStates = new List<bool>();

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("[RecorderManager] Player not assigned!");
            return;
        }

        // Auto-find components if not assigned
        if (playerSpriteRenderer == null)
        {
            playerSpriteRenderer = player.GetComponentInChildren<SpriteRenderer>();
        }
        if (playerAnimator == null)
        {
            playerAnimator = player.GetComponentInChildren<Animator>();
        }
        Debug.Log("[RecorderManager] Started recording player movements");
    }

    void Update()
    {
        if (isRecording && player != null)
        {
            positions.Add(player.transform.position);
            rotations.Add(player.transform.rotation);
            scales.Add(player.transform.localScale);
            flipXStates.Add(playerSpriteRenderer.flipX);
            isRunningStates.Add(playerAnimator.GetBool("isRunning"));
            isJumpingStates.Add(playerAnimator.GetBool("isJumping"));

            if (positions.Count % frameNumber == 0)
            {
                Debug.Log($"[RecorderManager] Recorded {positions.Count} frames | Current pos: {player.transform.position}");
            }
        }
    }

    public List<Vector2> GetPositions()
    {
        Debug.Log($"[RecorderManager] Returning {positions.Count} recorded positions");
        return positions;
    }

    public List<Quaternion> GetRotations()
    {
        Debug.Log($"[RecorderManager] Returning {rotations.Count} recorded rotations");
        return rotations;
    }

    public List<Vector2> GetScales()
    {
        Debug.Log($"[RecorderManager] Returning {scales.Count} recorded scales");
        return scales;
    }

    public List<bool> GetFlipXStates()
    {
        Debug.Log($"[RecorderManager] Returning {flipXStates.Count} recorded flipX states");
        return flipXStates;
    }

    // Getting the animations
    public List<bool> GetIsRunningStates() => isRunningStates;

    public List<bool> GetIsJumpingStates() => isJumpingStates;
    public void StopRecording()
    {
        isRecording = false;
        Debug.Log($"[RecorderManager] Stopped recording. Total frames: {positions.Count}");
    }

    public void ClearRecording()
    {
        positions.Clear();
        rotations.Clear();
        scales.Clear();
        flipXStates.Clear();
        isRunningStates.Clear();
        isJumpingStates.Clear();
        Debug.Log("[RecorderManager] Cleared all recorded data");
    }
}