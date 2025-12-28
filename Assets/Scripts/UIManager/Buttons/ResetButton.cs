using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetButton : MonoBehaviour
{
    [Header("Reset Options")]
    [Tooltip("If true, reloads current scene. If false, loads specific scene.")]
    [SerializeField] private bool reloadCurrentScene = true;

    [Tooltip("Scene to load if not reloading current (only used if reloadCurrentScene is false)")]
    [SerializeField] private string sceneToLoad = "";

    // Resets the current level
    // Call this from the Button's OnClick event
    public void ResetLevel()
    {
        Debug.Log("[ResetButton] Resetting level...");

        if (reloadCurrentScene)
        {
            if (DeathTracker.Instance != null)
            {
                DeathTracker.Instance.RecordDeath();
            }
            // Reload the current scene
            Scene currentScene = SceneManager.GetActiveScene();
            Debug.Log($"[ResetButton] Reloading scene: {currentScene.name}");
            SceneManager.LoadScene(currentScene.name);
        }
        else
        {
            // Load specific scene
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                Debug.Log($"[ResetButton] Loading scene: {sceneToLoad}");
                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                Debug.LogError("[ResetButton] Scene to load is not specified!");
            }
        }
    }

    // Optional: Reset by scene index
    public void ResetLevelByIndex(int sceneIndex)
    {
        Debug.Log($"[ResetButton] Loading scene by index: {sceneIndex}");

        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError($"[ResetButton] Invalid scene index: {sceneIndex}");
        }
    }
}