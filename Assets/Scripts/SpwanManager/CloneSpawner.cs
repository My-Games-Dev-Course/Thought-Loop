using UnityEngine;
using System.Collections.Generic;

public class CloneSpawner : MonoBehaviour
{
    [SerializeField] private RecorderManager recorderManager;
    [SerializeField] private GameObject clonePrefab;

    void Start()
    {
        if (recorderManager == null)
        {
            Debug.LogError("[CloneSpawner] RecorderManager not assigned!");
        }
    }

    public void SpawnClone()
    {
        // Stop recording
        recorderManager.StopRecording();

        // Get recorded data
        var positions = recorderManager.GetPositions();
        var rotations = recorderManager.GetRotations();
        var scales = recorderManager.GetScales();
        var flipXStates = recorderManager.GetFlipXStates();
        var isRunningStates = recorderManager.GetIsRunningStates();
        var isJumpingStates = recorderManager.GetIsJumpingStates();

        if (positions.Count > 0)
        {
            // Spawn clone at starting position
            GameObject clone = Instantiate(clonePrefab, positions[0], rotations[0]);

            // Start playback
            ClonePlayback playback = clone.GetComponent<ClonePlayback>();
            if (playback != null)
            {
                playback.StartPlayback(positions, rotations, scales, flipXStates, isRunningStates, isJumpingStates, playback.GetRb());
                Debug.Log("[CloneSpawner] Clone spawned and playback started!");
            }
            else
            {
                Debug.LogError("[CloneSpawner] ClonePlayback component not found on clone!");
            }
        }
        else
        {
            Debug.LogWarning("[CloneSpawner] No recorded positions to play back!");
        }
    }
}