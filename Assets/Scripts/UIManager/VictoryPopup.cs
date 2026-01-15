using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryPopup : MonoBehaviour
{
    public static VictoryPopup Instance { get; private set; }

    [Header("Popup References")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI congratsText;
    [SerializeField] private Button nextLevelButton;

    [Header("Popup Settings")]
    [SerializeField] private string congratsMessage = "Good Job!";
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
        // Make sure panel is actually visible before accepting Enter
        bool popupIsActive = popupPanel != null && popupPanel.activeSelf;

        // Only process Enter key when popup is fully showing
        if (isShowing && popupIsActive && Time.timeScale == 0f &&
            (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
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
    }

    public void ShowVictory(string sceneName = "", int sceneIndex = -1)
    {
        if (isShowing) return;

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

                // Death stats commented out for cleaner UI
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
    }

    public void HidePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        Time.timeScale = ResumeGame;
        isShowing = false;
    }
    // Check if victory popup is currently showing
    public bool IsShowing()
    {
        return isShowing;
    }

    private void OnNextLevelClicked()
    {
        StartCoroutine(NextLevelFlow());
    }

    private void DisableGameplayScripts()
    {
        // Stop all enemies
        EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();
        foreach (var enemy in enemies)
        {
            enemy.enabled = false;
        }

        // Stop player
        playerMovment player = FindObjectOfType<playerMovment>();
        if (player != null)
        {
            player.enabled = false;
        }
    }

    private IEnumerator NextLevelFlow()
    {
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

        // Save progress to cloud if signed in
        if (CloudSaveManager.Instance != null && CloudSaveManager.Instance.IsSignedIn())
        {
            var updateTask = CloudSaveManager.Instance.UpdateCurrentLevel(nextLevelIndex);

            while (!updateTask.IsCompleted)
            {
                yield return null;
            }
        }

        // Local backup save
        PlayerPrefs.SetInt("CurrentLevelIndex", nextLevelIndex);
        PlayerPrefs.Save();

        // Unpause before loading
        Time.timeScale = 1f;

        // Load next scene
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
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private void LoadSceneWithFade(int index)
    {
        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(index);
        }
        else
        {
            SceneManager.LoadScene(index);
        }
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}