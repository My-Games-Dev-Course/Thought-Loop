using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// Level select screen - shows available levels and lets player choose
// ALL LEVELS ARE ALWAYS UNLOCKED
public class LevelSelectManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform levelButtonContainer;
    [SerializeField] private Button backButton;

    [Header("Level Settings")]
    [SerializeField]
    private string[] levelNames = {
        "Tutorial1", "Tutorial2",
        "Level1", "Level2", "Level3",
        "Level4", "Level5", "Level6",
        "Level7", "Level8"
    };

    [SerializeField] private string mainMenuSceneName = "MainMenu";

    void Start()
    {
        CreateLevelButtons();

        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    // Create a button for each level - ALL UNLOCKED
    private void CreateLevelButtons()
    {
        for (int i = 0; i < levelNames.Length; i++)
        {
            // Always unlocked - no restrictions!
            CreateLevelButton(i, levelNames[i], true);
        }
    }

    // Create individual level button
    private void CreateLevelButton(int levelIndex, string levelName, bool isUnlocked)
    {
        if (levelButtonPrefab == null || levelButtonContainer == null)
        {
            Debug.LogError("Level button prefab or container not assigned!");
            return;
        }

        GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonContainer);

        // Get button component
        Button button = buttonObj.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Level button prefab doesn't have Button component!");
            return;
        }

        // Set button text
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = FormatLevelName(levelName);
        }

        // All levels are always clickable
        button.interactable = true;
        button.onClick.AddListener(() => OnLevelClicked(levelName));
    }

    // Format level name for display
    private string FormatLevelName(string levelName)
    {
        // Convert "Level1" to "Level 1", "Tutorial1" to "Tutorial 1", etc.
        string formatted = levelName;

        if (levelName.StartsWith("Level"))
        {
            formatted = "Level " + levelName.Substring(5);
        }
        else if (levelName.StartsWith("Tutorial"))
        {
            formatted = "Tutorial " + levelName.Substring(8);
        }

        return formatted;
    }

    // Load selected level
    private void OnLevelClicked(string levelName)
    {
        Debug.Log($"Loading level: {levelName}");
        LoadSceneWithFade(levelName);
    }

    // Return to main menu
    private void OnBackClicked()
    {
        LoadSceneWithFade(mainMenuSceneName);
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