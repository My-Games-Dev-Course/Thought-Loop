using UnityEngine;
using UnityEngine.SceneManagement;

public class CrushDetector : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag to detect (usually 'Player')")]
    [SerializeField] private string targetTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if Player touched the crush detector
        if (other.CompareTag(targetTag))
        {
            if (DeathTracker.Instance != null)
            {
                DeathTracker.Instance.RecordDeath();
            }

            Debug.Log("[CrushDetector] Player crushed! Restarting scene...");

            // Stop all movement immediately
            Time.timeScale = 0f;

            // Show death message
            if (DeathMessage.Instance != null)
            {
                DeathMessage.Instance.ShowDeathMessage("Crushed by falling wall!");
            }
            else
            {
                // Fallback if no death message system
                Time.timeScale = 1f;
                RestartScene();
            }
        }
    }

    private void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}