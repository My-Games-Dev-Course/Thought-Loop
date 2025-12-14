using TMPro;
using UnityEngine;

/**
 * This code manages the clone spawning timer system.
 * Each clone gets its own independent recording period.
 */
public class CloneTimerMult : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [Tooltip("Array of recording durations for each clone. Example: [10, 5] means Clone #1 records for 10s, Clone #2 for 5s")]
    [SerializeField] private float[] cloneTimings = new float[] { 10f }; // Default: single clone at 10s
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private CloneSpawnerMult cloneSpawner;

    private float currentTime;
    private bool timerRunning = true;
    private int currentCloneIndex = 0; // Track which clone we're spawning next

    void Start()
    {
        if (cloneTimings.Length == 0)
        {
            Debug.LogWarning("[CloneTimer] No clone timings set! Using default 10s.");
            cloneTimings = new float[] { 10f };
        }
        currentTime = cloneTimings[0];
        Debug.Log($"[CloneTimer] Starting timer with {cloneTimings.Length} clone(s)");
        Debug.Log($"[CloneTimer] Recording for Clone #1: {currentTime}s");
    }

    void Update()
    {
        if (timerRunning && currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                TimerEnded();
            }

            // Update timer text
            int cloneNumber = currentCloneIndex + 1;
            timerText.text = $"Clone in: {Mathf.CeilToInt(currentTime)}";
        }
    }

    void TimerEnded()
    {
        int cloneNumber = currentCloneIndex + 1;
        Debug.Log($"[CloneTimer] Timer ended for clone #{cloneNumber}");

        // Check if there are more clones after this one
        bool hasMoreClones = (currentCloneIndex < cloneTimings.Length - 1);

        // Spawn the clone
        // startNewRecording = true if more clones are coming (except for last one)
        cloneSpawner.SpawnClone(hasMoreClones);

        currentCloneIndex++;

        // Check if there are more clones to spawn
        if (currentCloneIndex < cloneTimings.Length)
        {
            // Start next timer for next clone
            currentTime = cloneTimings[currentCloneIndex];
            int nextCloneNumber = currentCloneIndex + 1;
            Debug.Log($"[CloneTimer] Starting recording for Clone #{nextCloneNumber}: {currentTime}s");
        }
        else
        {
            // All clones spawned - hide timer
            timerRunning = false;
            timerText.gameObject.SetActive(false);
            Debug.Log("[CloneTimer] All clones spawned! Recording complete.");
        }
    }
}