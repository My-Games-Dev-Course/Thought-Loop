//using TMPro;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;


//// Handles the victory popup that appears when reaching the flag
//// Pauses the game and allows player to proceed to next level

//public class VictoryPopup : MonoBehaviour
//{
//    public static VictoryPopup Instance { get; private set; }

//    [Header("Popup References")]
//    [SerializeField] private GameObject popupPanel;
//    [SerializeField] private TextMeshProUGUI congratsText;
//    [SerializeField] private Button nextLevelButton;

//    [Header("Popup Settings")]
//    [SerializeField] private string congratsMessage = "Congratulations!";
//    [SerializeField] private float ResumeGame = 1.0f;

//    [Header("Next Scene Settings")]
//    [SerializeField] private bool useBuildIndex = false;
//    [SerializeField] private int nextSceneIndex = 1;
//    [SerializeField] private string nextSceneName = "";

//    private bool isShowing = false;

//    void Awake()
//    {
//        // Singleton pattern
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//            return;
//        }

//        // Reset state on scene load
//        ResetPopupState();
//    }

//    private void ResetPopupState()
//    {
//        // Reset showing state
//        isShowing = false;

//        // Make sure game is not paused
//        Time.timeScale = 1f;

//        // Make sure popup is hidden
//        if (popupPanel != null)
//        {
//            popupPanel.SetActive(false);
//        }

//        // Setup button listener
//        if (nextLevelButton != null)
//        {
//            nextLevelButton.onClick.RemoveAllListeners(); // Clear old listeners
//            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
//        }

//        Debug.Log("[VictoryPopup] Popup state reset");
//    }


//    // Show the victory popup and pause the game

//    public void ShowVictory(string sceneName = "", int sceneIndex = -1)
//    {
//        if (isShowing) return;

//        Debug.Log("[VictoryPopup] Showing victory popup!");

//        // Store next scene info if provided
//        if (!string.IsNullOrEmpty(sceneName))
//        {
//            nextSceneName = sceneName;
//            useBuildIndex = false;
//        }
//        else if (sceneIndex >= 0)
//        {
//            nextSceneIndex = sceneIndex;
//            useBuildIndex = true;
//        }

//        // Set texts
//        if (congratsText != null)
//        {
//            congratsText.text = congratsMessage;
//        }

//        // Show popup
//        if (popupPanel != null)
//        {
//            popupPanel.SetActive(true);
//        }

//        // PAUSE THE GAME
//        Time.timeScale = 0f;
//        isShowing = true;

//        Debug.Log("[VictoryPopup] Game paused. Time.timeScale = 0");
//    }


//    // Hide the popup and resume the game

//    public void HidePopup()
//    {
//        if (popupPanel != null)
//        {
//            popupPanel.SetActive(false);
//        }

//        // RESUME THE GAME
//        Time.timeScale = ResumeGame;
//        isShowing = false;

//        Debug.Log("[VictoryPopup] Game resumed. Time.timeScale = 1");
//    }


//    //// Called when the "Next Level" button is clicked

//    //private void OnNextLevelClicked()
//    //{
//    //    Debug.Log("[VictoryPopup] Next Level button clicked!");

//    //    // Resume game before loading next scene
//    //    Time.timeScale = 1f;

//    //    // Load next scene with fade
//    //    if (useBuildIndex)
//    //    {
//    //        LoadSceneWithFade(nextSceneIndex);
//    //    }
//    //    else
//    //    {
//    //        LoadSceneWithFade(nextSceneName);
//    //    }
//    //}

//    /**
//     * Called when the "Next Level" button is clicked
//     * NOW SAVES PROGRESS TO CLOUD USING BUILD INDEX!
//     */
//    private async void OnNextLevelClicked()
//    {
//        Debug.Log("[VictoryPopup] Next Level button clicked!");

//        // Resume game before loading next scene
//        Time.timeScale = 1f;

//        // ========== CLOUD SAVE INTEGRATION ==========
//        // Get current scene build index
//        Scene currentScene = SceneManager.GetActiveScene();
//        int currentBuildIndex = currentScene.buildIndex;

//        // Calculate next level build index
//        int nextLevelIndex;
//        if (useBuildIndex)
//        {
//            nextLevelIndex = nextSceneIndex;
//        }
//        else
//        {
//            // If loading by name, get its build index
//            nextLevelIndex = SceneUtility.GetBuildIndexByScenePath(nextSceneName);
//            if (nextLevelIndex == -1)
//            {
//                // Fallback: use current + 1
//                nextLevelIndex = currentBuildIndex + 1;
//            }
//        }

//        Debug.Log($"[VictoryPopup] Current level: {currentBuildIndex}, Next level: {nextLevelIndex}");

//        // Save to cloud if signed in
//        if (CloudSaveManager.Instance != null && CloudSaveManager.Instance.IsSignedIn())
//        {
//            Debug.Log($"[VictoryPopup] Saving progress: Level index {nextLevelIndex} to cloud");
//            await CloudSaveManager.Instance.SaveCurrentLevel(nextLevelIndex);
//        }
//        else
//        {
//            Debug.LogWarning("[VictoryPopup] Not signed in - progress won't be saved to cloud");
//        }

//        // Also save locally as backup
//        PlayerPrefs.SetInt("CurrentLevelIndex", nextLevelIndex);
//        PlayerPrefs.Save();
//        Debug.Log($"[VictoryPopup] Saved level index {nextLevelIndex} locally");
//        // ========== END CLOUD SAVE ==========

//        // Load next scene with fade
//        if (useBuildIndex)
//        {
//            LoadSceneWithFade(nextSceneIndex);
//        }
//        else
//        {
//            LoadSceneWithFade(nextSceneName);
//        }
//    }

//    // Load scene with fade effect (by name)

//    private void LoadSceneWithFade(string sceneName)
//    {
//        if (SceneFader.Instance != null)
//        {
//            SceneFader.Instance.FadeToScene(sceneName);
//            Debug.Log($"[VictoryPopup] Loading scene '{sceneName}' with fade");
//        }
//        else
//        {
//            // Fallback without fade
//            Debug.LogWarning("[VictoryPopup] SceneFader not found! Loading without fade.");
//            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
//        }
//    }


//    // Load scene with fade effect (by index)

//    private void LoadSceneWithFade(int index)
//    {
//        if (SceneFader.Instance != null)
//        {
//            SceneFader.Instance.FadeToScene(index);
//            Debug.Log($"[VictoryPopup] Loading scene index {index} with fade");
//        }
//        else
//        {
//            // Fallback without fade
//            Debug.LogWarning("[VictoryPopup] SceneFader not found! Loading without fade.");
//            UnityEngine.SceneManagement.SceneManager.LoadScene(index);
//        }
//    }

//    void OnDestroy()
//    {
//        // Make sure game is unpaused if popup is destroyed
//        Time.timeScale = 1f;
//    }
//}


using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Handles the victory popup that appears when reaching the flag
// Pauses the game and allows player to proceed to next level
// NOW WITH CLOUD SAVE + PLAYER DATA TRACKING

public class VictoryPopup : MonoBehaviour
{
    public static VictoryPopup Instance { get; private set; }

    [Header("Popup References")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI congratsText;
    [SerializeField] private Button nextLevelButton;

    [Header("Popup Settings")]
    [SerializeField] private string congratsMessage = "Congratulations!";
    [SerializeField] private float ResumeGame = 1.0f;

    [Header("Next Scene Settings")]
    [SerializeField] private bool useBuildIndex = false;
    [SerializeField] private int nextSceneIndex = 1;
    [SerializeField] private string nextSceneName = "";

    private bool isShowing = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Reset state on scene load
        ResetPopupState();
    }

    private void ResetPopupState()
    {
        // Reset showing state
        isShowing = false;

        // Make sure game is not paused
        Time.timeScale = 1f;

        // Make sure popup is hidden
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        // Setup button listener
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners(); // Clear old listeners
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        }

        Debug.Log("[VictoryPopup] Popup state reset");
    }

    /// <summary>
    /// Show the victory popup and pause the game
    /// </summary>
    public void ShowVictory(string sceneName = "", int sceneIndex = -1)
    {
        if (isShowing) return;

        Debug.Log("[VictoryPopup] Showing victory popup!");

        // Store next scene info if provided
        if (!string.IsNullOrEmpty(sceneName))
        {
            nextSceneName = sceneName;
            useBuildIndex = false;
        }
        else if (sceneIndex >= 0)
        {
            nextSceneIndex = sceneIndex;
            useBuildIndex = true;
        }

        // Set texts
        if (congratsText != null)
        {
            // Show level completion message with death stats
            string message = congratsMessage;

            // Add death count if CloudSaveManager is available
            if (CloudSaveManager.Instance != null)
            {
                int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
                int deathsThisLevel = CloudSaveManager.Instance.GetFailsForLevel(currentLevelIndex);
                int currentGameDeaths = CloudSaveManager.Instance.GetCurrentGameDeaths();
                int bestScore = CloudSaveManager.Instance.GetBestScore();

                //message += $"\n\nDeaths this level: {deathsThisLevel}";
                //message += $"\nCurrent game total: {currentGameDeaths}";

                if (bestScore != -1)
                {
                    //message += $"\n🏆 Best score: {bestScore}";
                }
            }

            congratsText.text = message;
        }

        // Show popup
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }

        // PAUSE THE GAME
        Time.timeScale = 0f;
        isShowing = true;

        Debug.Log("[VictoryPopup] Game paused. Time.timeScale = 0");
    }

    /// <summary>
    /// Hide the popup and resume the game
    /// </summary>
    public void HidePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        // RESUME THE GAME
        Time.timeScale = ResumeGame;
        isShowing = false;

        Debug.Log("[VictoryPopup] Game resumed. Time.timeScale = 1");
    }

    /// <summary>
    /// Called when the "Next Level" button is clicked
    /// SAVES PROGRESS TO CLOUD USING PLAYER DATA SYSTEM
    /// </summary>
    private async void OnNextLevelClicked()
    {
        Debug.Log("[VictoryPopup] Next Level button clicked!");

        // Resume game before loading next scene
        Time.timeScale = 1f;

        // ========== CLOUD SAVE INTEGRATION WITH PLAYER DATA ==========
        // Get current scene build index
        Scene currentScene = SceneManager.GetActiveScene();
        int currentBuildIndex = currentScene.buildIndex;

        // Calculate next level build index
        int nextLevelIndex;
        if (useBuildIndex)
        {
            nextLevelIndex = nextSceneIndex;
        }
        else
        {
            // If loading by name, get its build index
            nextLevelIndex = SceneUtility.GetBuildIndexByScenePath(nextSceneName);
            if (nextLevelIndex == -1)
            {
                // Fallback: use current + 1
                nextLevelIndex = currentBuildIndex + 1;
            }
        }

        Debug.Log($"[VictoryPopup] Current level: {currentBuildIndex}, Next level: {nextLevelIndex}");

        // Save progress to cloud (just update current level, NOT complete game)
        if (CloudSaveManager.Instance != null && CloudSaveManager.Instance.IsSignedIn())
        {
            Debug.Log($"[VictoryPopup] Saving progress to cloud...");
            await CloudSaveManager.Instance.UpdateCurrentLevel(nextLevelIndex);
            Debug.Log($"[VictoryPopup] ✓ Progress saved! Next level: {nextLevelIndex}");
        }
        else
        {
            Debug.LogWarning("[VictoryPopup] Not signed in - progress won't be saved to cloud");
        }

        // Also save locally as backup
        PlayerPrefs.SetInt("CurrentLevelIndex", nextLevelIndex);
        PlayerPrefs.Save();
        Debug.Log($"[VictoryPopup] Saved level index {nextLevelIndex} to PlayerPrefs");
        // ========== END CLOUD SAVE ==========

        // Load next scene with fade
        if (useBuildIndex)
        {
            LoadSceneWithFade(nextSceneIndex);
        }
        else
        {
            LoadSceneWithFade(nextSceneName);
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
            Debug.Log($"[VictoryPopup] Loading scene '{sceneName}' with fade");
        }
        else
        {
            // Fallback without fade
            Debug.LogWarning("[VictoryPopup] SceneFader not found! Loading without fade.");
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
            Debug.Log($"[VictoryPopup] Loading scene index {index} with fade");
        }
        else
        {
            // Fallback without fade
            Debug.LogWarning("[VictoryPopup] SceneFader not found! Loading without fade.");
            SceneManager.LoadScene(index);
        }
    }

    void OnDestroy()
    {
        // Make sure game is unpaused if popup is destroyed
        Time.timeScale = 1f;
    }
}