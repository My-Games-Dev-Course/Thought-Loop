using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Main menu manager - handles play, skip tutorial, level select, and logout
public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button skipTutorialButton;
    [SerializeField] private Button levelSelectButton;
    [SerializeField] private Button logoutButton;

    [Header("Scene Names")]
    [SerializeField] private string tutorialSceneName = "Tutorial1";
    [SerializeField] private string firstLevelName = "Level1";
    [SerializeField] private string levelSelectSceneName = "LevelSelect";
    [SerializeField] private string loginSceneName = "LoginScene";

    void Start()
    {
        // Connect all buttons to their functions
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClicked);
        }

        if (skipTutorialButton != null)
        {
            skipTutorialButton.onClick.AddListener(OnSkipTutorialClicked);
        }

        if (levelSelectButton != null)
        {
            levelSelectButton.onClick.AddListener(OnLevelSelectClicked);
        }

        if (logoutButton != null)
        {
            logoutButton.onClick.AddListener(OnLogoutClicked);
        }
    }

    // Start game from tutorial
    private void OnPlayClicked()
    {
        // Just go to tutorial - don't reset progress
        LoadSceneWithFade(tutorialSceneName);
    }

    // Skip tutorial and start from Level 1
    private void OnSkipTutorialClicked()
    {
        // Just go to Level 1 - don't reset progress
        LoadSceneWithFade(firstLevelName);
    }

    // Open level select screen
    private void OnLevelSelectClicked()
    {
        LoadSceneWithFade(levelSelectSceneName);
    }

    // Logout and return to login screen
    private void OnLogoutClicked()
    {
        // Sign out from cloud save if signed in
        if (CloudSaveManager.Instance != null && CloudSaveManager.Instance.IsSignedIn())
        {
            // Cloud logout handled by LoginUI when returning to login scene
        }

        // Clear any local session data if needed
        PlayerPrefs.DeleteKey("IsLoggedIn");
        PlayerPrefs.Save();

        LoadSceneWithFade(loginSceneName);
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
}