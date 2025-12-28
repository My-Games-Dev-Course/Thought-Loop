using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * Exit Game Button - Returns to main menu
 * If this is the FINAL LEVEL, completes game and updates best score BEFORE exiting
 */
public class NewGameButton : MonoBehaviour
{
    [Header("Scene to Load")]
    [Tooltip("Name of the main menu scene")]
    [SerializeField] private string firstSceneName = "Main Menu";

    [Header("Alternative: Use Build Index")]
    [Tooltip("If you prefer to use build index instead of name")]
    [SerializeField] private bool useBuildIndex = true;
    [SerializeField] private int sceneIndex = 1; // Main Menu

    [Header("Final Level Detection")]
    [Tooltip("Build index of the LAST playable level (e.g., 5 for Level3)")]
    [SerializeField] private int finalLevelIndex = 11; // Change this to your last level!

    private Button button;

    void Start()
    {
        // Get the Button component
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnExitButtonClicked);
            Debug.Log("[ExitButton] Button connected");
        }
        else
        {
            Debug.LogError("[ExitButton] No Button component found!");
        }
    }

    /// <summary>
    /// Called when Exit button is clicked
    /// Completes game if this is the final level, then returns to menu
    /// </summary>
    async void OnExitButtonClicked()
    {
        Debug.Log("[ExitButton] Exit button clicked");

        // Check if this is the final level
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentLevelIndex == finalLevelIndex)
        {
            // THIS IS THE FINAL LEVEL!
            Debug.Log("[ExitButton] 🏆 FINAL LEVEL - Completing game...");

            if (CloudSaveManager.Instance != null && CloudSaveManager.Instance.IsSignedIn())
            {
                int deathsBefore = CloudSaveManager.Instance.GetCurrentGameDeaths();
                int bestBefore = CloudSaveManager.Instance.GetBestScore();

                Debug.Log($"[ExitButton] Deaths: {deathsBefore}, Best: {bestBefore}");

                // WAIT FOR COMPLETE GAME TO FINISH!
                await CloudSaveManager.Instance.CompleteGame();

                // Wait a bit more to ensure cloud save completes
                await System.Threading.Tasks.Task.Delay(500);

                int bestAfter = CloudSaveManager.Instance.GetBestScore();
                Debug.Log($"[ExitButton] ✓ Saved! Best: {bestBefore}→{bestAfter}");
            }
        }

        // NOW load menu (after save completes)
        ExitToFirstScene();
    }

    /// <summary>
    /// Load the main menu scene
    /// </summary>
    public void ExitToFirstScene()
    {
        if (useBuildIndex)
        {
            // Load by build index WITH FADE
            if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                LoadSceneWithFade(sceneIndex);
            }
            else
            {
                Debug.LogError($"[ExitButton] Invalid scene index: {sceneIndex}");
            }
        }
        else
        {
            // Load by scene name WITH FADE
            if (!string.IsNullOrEmpty(firstSceneName))
            {
                LoadSceneWithFade(firstSceneName);
            }
            else
            {
                Debug.LogError("[ExitButton] Scene name is empty!");
            }
        }
    }

    /// <summary>
    /// Load scene with fade effect (by name)
    /// </summary>
    private void LoadSceneWithFade(string sceneName)
    {
        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(sceneName);
            Debug.Log($"[ExitButton] Loading scene '{sceneName}' with fade");
        }
        else
        {
            // Fallback: direct load without fade
            Debug.LogWarning("[ExitButton] SceneFader not found! Loading without fade.");
            SceneManager.LoadScene(sceneName);
        }
    }

    /// <summary>
    /// Load scene with fade effect (by index)
    /// </summary>
    private void LoadSceneWithFade(int index)
    {
        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(index);
            Debug.Log($"[ExitButton] Loading scene index {index} with fade");
        }
        else
        {
            // Fallback: direct load without fade
            Debug.LogWarning("[ExitButton] SceneFader not found! Loading without fade.");
            SceneManager.LoadScene(index);
        }
    }

    void OnDestroy()
    {
        // Clean up the listener
        if (button != null)
        {
            button.onClick.RemoveListener(OnExitButtonClicked);
        }
    }
}