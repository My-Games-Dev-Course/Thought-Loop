using UnityEngine;
using UnityEngine.UI;
using TMPro;

/**
 * Circular visual timer that supports multiple clones with array of timings.
 * Combines the visual circle countdown with multi-clone functionality.
 */
public class CircularTimerMult : MonoBehaviour
{
    [Header("UI Elements")]
    public Image circleImage;
    public TextMeshProUGUI numberText;

    [Header("Clone Timings")]
    [Tooltip("Array of recording durations for each clone. Example: [10, 5, 3] means Clone #1: 10s, Clone #2: 5s, Clone #3: 3s")]
    public float[] cloneTimings = new float[] { 10f, 10f }; // Default: 2 clones at 10s each

    [Header("Spawner Reference")]
    public CloneSpawnerMult cloneSpawner;

    private float currentTime;
    private float currentMaxTime; // The max time for the current clone
    private bool isRunning = false;
    private int currentCloneIndex = 0;

    void Start()
    {
        // Validation
        if (cloneSpawner == null)
        {
            Debug.LogError("[CircularTimerMult] CloneSpawnerMult not assigned!");
            enabled = false;
            return;
        }

        if (cloneTimings.Length == 0)
        {
            Debug.LogWarning("[CircularTimerMult] No clone timings set! Using default [10].");
            cloneTimings = new float[] { 10f };
        }

        // Start first timer
        currentMaxTime = cloneTimings[0];
        currentTime = currentMaxTime;
        isRunning = true;

        Debug.Log($"[CircularTimerMult] Starting timer with {cloneTimings.Length} clone(s)");
        Debug.Log($"[CircularTimerMult] Recording for Clone #1: {currentTime}s");

        UpdateDisplay();
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            TimeUp();
        }

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        // Update circular fill (normalized to current clone's max time)
        if (circleImage != null)
        {
            circleImage.fillAmount = currentTime / currentMaxTime;
        }

        // Update number text
        if (numberText != null)
        {
            int displayTime = Mathf.CeilToInt(currentTime);
            numberText.text = displayTime.ToString();
        }
    }

    void TimeUp()
    {
        int cloneNumber = currentCloneIndex + 1;
        Debug.Log($"[CircularTimerMult] Timer ended for clone #{cloneNumber}");

        isRunning = false;

        // Check if there are more clones after this one
        bool hasMoreClones = (currentCloneIndex < cloneTimings.Length - 1);

        // Spawn the clone
        cloneSpawner.SpawnClone(hasMoreClones);

        currentCloneIndex++;

        // Check if there are more clones to spawn
        if (currentCloneIndex < cloneTimings.Length)
        {
            // Start next timer for next clone
            currentMaxTime = cloneTimings[currentCloneIndex];
            currentTime = currentMaxTime;
            isRunning = true;

            int nextCloneNumber = currentCloneIndex + 1;
            Debug.Log($"[CircularTimerMult] Starting recording for Clone #{nextCloneNumber}: {currentTime}s");

            UpdateDisplay();
        }
        else
        {
            // All clones spawned - hide timer
            Debug.Log("[CircularTimerMult] All clones spawned! Recording complete.");
            gameObject.SetActive(false);
        }
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    public void ResetTimer()
    {
        currentCloneIndex = 0;
        currentMaxTime = cloneTimings[0];
        currentTime = currentMaxTime;
        isRunning = true;
        gameObject.SetActive(true);
        UpdateDisplay();
        Debug.Log("[CircularTimerMult] Timer reset to beginning");
    }
}