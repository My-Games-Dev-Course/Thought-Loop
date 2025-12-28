using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Death Tracker - Records player deaths/fails to CloudSave
 * 
 * HOW TO USE:
 * 1. Add this script to your Player GameObject
 * 2. Call RecordDeath() whenever the player dies
 * 
 * Example:
 * void OnPlayerDeath() {
 *     DeathTracker.Instance.RecordDeath();
 * }
 */
public class DeathTracker : MonoBehaviour
{
    public static DeathTracker Instance { get; private set; }

    private int currentLevelIndex;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("[DeathTracker] Duplicate instance found, destroying");
            Destroy(gameObject);
            return;
        }

        // Get current level index
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log($"[DeathTracker] Initialized for level {currentLevelIndex}");
    }

    /**
     * Call this method whenever the player dies/fails
     */
    //public async void RecordDeath()
    //{
    //    if (CloudSaveManager.Instance == null)
    //    {
    //        Debug.LogWarning("[DeathTracker] CloudSaveManager not found!");
    //        return;
    //    }

    //    Debug.Log($"[DeathTracker] Recording death in level {currentLevelIndex}");
    //    await CloudSaveManager.Instance.RecordFail(currentLevelIndex);
    //}

    public void RecordDeath()
    {
        if (CloudSaveManager.Instance == null)
        {
            Debug.LogWarning("[DeathTracker] CloudSaveManager not found!");
            return;
        }

        Debug.Log($"[DeathTracker] Recording death in level {currentLevelIndex}");
        StartCoroutine(RecordDeathFlow());
    }

    private IEnumerator RecordDeathFlow()
    {
        var recordTask = CloudSaveManager.Instance.RecordFail(currentLevelIndex);
        yield return new WaitUntil(() => recordTask.IsCompleted);

        Debug.Log($"[DeathTracker] ✓ Death recorded");
    }

    /**
     * Get total deaths in current level
     */
    public int GetDeathsThisLevel()
    {
        if (CloudSaveManager.Instance != null)
        {
            return CloudSaveManager.Instance.GetFailsForLevel(currentLevelIndex);
        }
        return 0;
    }

    /**
     * Optional: Display death count on screen
     */
    public void DisplayDeathCount()
    {
        int deaths = GetDeathsThisLevel();
        Debug.Log($"[DeathTracker] Deaths this level: {deaths}");
    }
}