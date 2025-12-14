using UnityEngine;
using System.Collections.Generic;

public class CloneSpawnerMult : MonoBehaviour
{
    [SerializeField] private RecorderManagerMult recorderManager;
    [SerializeField] private GameObject clonePrefab;

    private int cloneCount = 0;

    void Start()
    {
        if (recorderManager == null)
        {
            Debug.LogError("[CloneSpawner] RecorderManager not assigned!");
        }
        if (clonePrefab == null)
        {
            Debug.LogError("[CloneSpawner] Clone Prefab not assigned!");
        }
    }

    public void SpawnClone(bool startNewRecording = false)
    {
        cloneCount++;
        Debug.Log($"========== SPAWNING CLONE #{cloneCount} ==========");

        // STEP 1: Stop recording immediately
        recorderManager.StopRecording();
        Debug.Log("[CloneSpawner] Step 1: Recording stopped");

        // STEP 2: Get COPIES of all recorded data
        List<Vector2> positions = recorderManager.GetPositions();
        List<Quaternion> rotations = recorderManager.GetRotations();
        List<Vector2> scales = recorderManager.GetScales();
        List<bool> flipXStates = recorderManager.GetFlipXStates();
        List<bool> isRunningStates = recorderManager.GetIsRunningStates();
        List<bool> isJumpingStates = recorderManager.GetIsJumpingStates();
        Debug.Log($"[CloneSpawner] Step 2: Got {positions.Count} frames of data");

        // STEP 3: Validate we have data
        if (positions.Count == 0)
        {
            Debug.LogError("[CloneSpawner] ERROR: No recorded positions! Cannot spawn clone.");
            return;
        }

        // STEP 4: Log what we're about to do
        Vector2 spawnPosition = positions[0];
        Quaternion spawnRotation = rotations[0];
        Debug.Log($"[CloneSpawner] Step 3: Will spawn at position {spawnPosition}, rotation {spawnRotation}");

        // STEP 5: Spawn the clone at the FIRST recorded position
        GameObject clone = Instantiate(clonePrefab, spawnPosition, spawnRotation);
        clone.name = $"Clone #{cloneCount}";
        Debug.Log($"[CloneSpawner] Step 4: Clone instantiated at {clone.transform.position}");

        // STEP 5.5: Set clone to "Clone" layer so clones don't collide with each other
        int cloneLayer = LayerMask.NameToLayer("Clone");
        if (cloneLayer != -1)
        {
            clone.layer = cloneLayer;
            Debug.Log($"[CloneSpawner] Clone set to layer: Clone (layer {cloneLayer})");
        }
        else
        {
            Debug.LogWarning("[CloneSpawner] 'Clone' layer not found! Clones may collide with each other. Please create a 'Clone' layer in Project Settings.");
        }

        // STEP 5.6: Make clone KINEMATIC so it's not affected by physics forces
        Rigidbody2D cloneRb = clone.GetComponent<Rigidbody2D>();
        if (cloneRb != null)
        {
            cloneRb.bodyType = RigidbodyType2D.Kinematic;
            cloneRb.simulated = true; // Keep physics active for triggers (buttons)
            Debug.Log($"[CloneSpawner] Clone Rigidbody2D set to Kinematic");
        }
        else
        {
            Debug.LogWarning("[CloneSpawner] No Rigidbody2D found on clone - clone may not interact with triggers properly");
        }

        // STEP 6: Give the clone its playback component and data
        ClonePlayback playback = clone.GetComponent<ClonePlayback>();
        if (playback == null)
        {
            Debug.LogError("[CloneSpawner] ERROR: ClonePlayback component not found on clone!");
            return;
        }

        playback.StartPlayback(positions, rotations, scales, flipXStates, isRunningStates, isJumpingStates,
        playback.GetRb());
        Debug.Log($"[CloneSpawner] Step 5: Playback started with {positions.Count} frames");

        // STEP 7: Now that clone has its data, decide about recording
        if (startNewRecording)
        {
            Debug.Log("[CloneSpawner] Step 6: Clearing old recording and starting fresh");
            recorderManager.ClearRecording();
            recorderManager.StartRecording();
        }
        else
        {
            Debug.Log("[CloneSpawner] Step 6: Last clone - recording stays stopped");
        }

        Debug.Log($"========== CLONE #{cloneCount} SPAWN COMPLETE ==========");
    }
}