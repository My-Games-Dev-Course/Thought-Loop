using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CircularTimer : MonoBehaviour
{
    [Header("UI Elements")]
    public Image circleImage;
    public TextMeshProUGUI numberText;

    [Header("Timer Settings")]
    public float startTime = 10f;

    [Header("For Single Clone Levels (1-3)")]
    public CloneSpawner cloneSpawner;

    [Header("For Multiple Clone Levels (4-8)")]
    public CloneSpawnerMult cloneSpawnerMult;
    [Tooltip("Total number of clones for this level")]
    public int totalClones = 2;

    private float currentTime;
    private bool isRunning = false;
    private int currentCloneIndex = 0;

    void Start()
    {
        // Ensure at least one spawner is assigned
        if (cloneSpawner == null && cloneSpawnerMult == null)
        {
            Debug.LogError("CircularTimer: No spawner connected");
            enabled = false;
            return;
        }

        // Prioritize multi-clone spawner if both are assigned
        if (cloneSpawner != null && cloneSpawnerMult != null)
        {
            Debug.LogWarning("CircularTimer: Both spawners connected, using CloneSpawnerMult");
            cloneSpawner = null;
        }

        // Log initialization
        if (cloneSpawnerMult != null)
        {
            Debug.Log($"CircularTimer: Multi-clone mode - {totalClones} clones");
        }
        else
        {
            Debug.Log("CircularTimer: Single-clone mode");
        }

        ResetTimer();
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
        // Update visual timer circle
        if (circleImage != null)
        {
            circleImage.fillAmount = currentTime / startTime;
        }

        // Update numeric countdown text
        if (numberText != null)
        {
            int displayTime = Mathf.CeilToInt(currentTime);
            numberText.text = displayTime.ToString();
        }
    }

    void TimeUp()
    {
        isRunning = false;

        // Single clone 
        if (cloneSpawner != null)
        {
            Debug.Log("CircularTimer: Spawning single clone");
            cloneSpawner.SpawnClone();
            gameObject.SetActive(false);
            return;
        }

        // Multiple clone 
        if (cloneSpawnerMult != null)
        {
            int spawnNumber = currentCloneIndex + 1;

            Debug.Log($"CircularTimer: Spawning clone #{spawnNumber} of {totalClones}");

            // Determine if additional clones will spawn after this one
            bool hasMoreClones = (spawnNumber < totalClones);

            Debug.Log($"CircularTimer: hasMoreClones = {hasMoreClones}");

            // Trigger clone spawn
            cloneSpawnerMult.SpawnClone(hasMoreClones);

            currentCloneIndex++;

            // Continue timer for remaining clones
            if (hasMoreClones)
            {
                Debug.Log($"CircularTimer: Resetting for clone #{currentCloneIndex + 1}");
                ResetTimer();
            }
            else
            {
                // All clones spawned
                Debug.Log("CircularTimer: All clones spawned");
                gameObject.SetActive(false);
            }
        }
    }

    public void ResetTimer()
    {
        currentTime = startTime;
        isRunning = true;
        UpdateDisplay();
        gameObject.SetActive(true);

        if (cloneSpawnerMult != null)
        {
            Debug.Log($"CircularTimer: Reset to {startTime}s (clone #{currentCloneIndex + 1})");
        }
        else
        {
            Debug.Log($"CircularTimer: Reset to {startTime}s");
        }
    }

    public void PauseTimer()
    {
        isRunning = false;
        Debug.Log("CircularTimer: Paused");
    }

    public void ResumeTimer()
    {
        isRunning = true;
        Debug.Log("CircularTimer: Resumed");
    }
}