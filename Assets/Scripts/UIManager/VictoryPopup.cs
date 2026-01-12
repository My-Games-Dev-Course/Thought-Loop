using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Shows victory popup when player completes a level
// Handles cloud save and scene transitions
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
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ResetPopupState();
    }

    void Update()
    {
        // Check if Enter key was pressed while popup is showing
        if (isShowing && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            Debug.Log("[VictoryPopup] Enter key pressed - proceeding to next level");
            OnNextLevelClicked();
        }
    }

    private void ResetPopupState()
    {
        isShowing = false;
        Time.timeScale = 1f;

        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        }

        Debug.Log("[VictoryPopup] Popup state reset");
    }

    public void ShowVictory(string sceneName = "", int sceneIndex = -1)
    {
        if (isShowing) return;

        Debug.Log("[VictoryPopup] Showing victory popup!");

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

        if (congratsText != null)
        {
            string message = congratsMessage;

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

        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }

        Time.timeScale = 0f;
        isShowing = true;

        Debug.Log("[VictoryPopup] Game paused. Time.timeScale = 0");
    }

    public void HidePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        Time.timeScale = ResumeGame;
        isShowing = false;

        Debug.Log("[VictoryPopup] Game resumed. Time.timeScale = 1");
    }

    private void OnNextLevelClicked()
    {
        Debug.Log("[VictoryPopup] Next Level button clicked!");
        StartCoroutine(NextLevelFlow());
    }

    // Stop enemies and player from moving during scene transition
    private void DisableGameplayScripts()
    {
        EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();
        foreach (var enemy in enemies)
        {
            enemy.enabled = false;
        }

        playerMovment player = FindObjectOfType<playerMovment>();
        if (player != null)
        {
            player.enabled = false;
        }

        Debug.Log($"[VictoryPopup] Disabled {enemies.Length} enemies and player");
    }

    private IEnumerator NextLevelFlow()
    {
        // Make sure nothing can move or attack during transition
        DisableGameplayScripts();

        Scene currentScene = SceneManager.GetActiveScene();
        int currentBuildIndex = currentScene.buildIndex;

        int nextLevelIndex;
        if (useBuildIndex)
        {
            nextLevelIndex = nextSceneIndex;
        }
        else
        {
            nextLevelIndex = SceneUtility.GetBuildIndexByScenePath(nextSceneName);
            if (nextLevelIndex == -1)
            {
                nextLevelIndex = currentBuildIndex + 1;
            }
        }

        Debug.Log($"[VictoryPopup] Current level: {currentBuildIndex}, Next level: {nextLevelIndex}");

        // Wait for cloud save to finish
        if (CloudSaveManager.Instance != null && CloudSaveManager.Instance.IsSignedIn())
        {
            Debug.Log($"[VictoryPopup] Saving progress to cloud...");

            var updateTask = CloudSaveManager.Instance.UpdateCurrentLevel(nextLevelIndex);

            while (!updateTask.IsCompleted)
            {
                yield return null;
            }

            Debug.Log($"[VictoryPopup] Progress saved! Next level: {nextLevelIndex}");
        }
        else
        {
            Debug.LogWarning("[VictoryPopup] Not signed in - progress won't be saved to cloud");
        }

        // Also save locally as backup
        PlayerPrefs.SetInt("CurrentLevelIndex", nextLevelIndex);
        PlayerPrefs.Save();
        Debug.Log($"[VictoryPopup] Saved level index {nextLevelIndex} to PlayerPrefs");

        // Now unpause the game right before loading
        Time.timeScale = 1f;
        Debug.Log("[VictoryPopup] Game unpaused - loading scene now");

        // Load scene
        if (useBuildIndex)
        {
            LoadSceneWithFade(nextSceneIndex);
        }
        else
        {
            LoadSceneWithFade(nextSceneName);
        }
    }

    private void LoadSceneWithFade(string sceneName)
    {
        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(sceneName);
            Debug.Log($"[VictoryPopup] Loading scene '{sceneName}' with fade");
        }
        else
        {
            Debug.LogWarning("[VictoryPopup] SceneFader not found! Loading without fade.");
            SceneManager.LoadScene(sceneName);
        }
    }

    private void LoadSceneWithFade(int index)
    {
        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(index);
            Debug.Log($"[VictoryPopup] Loading scene index {index} with fade");
        }
        else
        {
            Debug.LogWarning("[VictoryPopup] SceneFader not found! Loading without fade.");
            SceneManager.LoadScene(index);
        }
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}