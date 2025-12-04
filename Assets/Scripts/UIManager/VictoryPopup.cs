using UnityEngine;
using UnityEngine.UI;
using TMPro;


// Handles the victory popup that appears when reaching the flag
// Pauses the game and allows player to proceed to next level

public class VictoryPopup : MonoBehaviour
{
    public static VictoryPopup Instance { get; private set; }

    [Header("Popup References")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI congratsText;
    [SerializeField] private Button nextLevelButton;

    [Header("Popup Settings")]
    [SerializeField] private string congratsMessage = "Congratulations!";
    [SerializeField] private float ResumeGame  = 1.0f;

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

        // Make sure popup is hidden at start
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        // Setup button listener
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        }
    }

    
    // Show the victory popup and pause the game
    
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
            congratsText.text = congratsMessage;
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

    
    // Hide the popup and resume the game
    
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

    
    /// Called when the "Next Level" button is clicked
    
    private void OnNextLevelClicked()
    {
        Debug.Log("[VictoryPopup] Next Level button clicked!");

        // Resume game before loading next scene
        Time.timeScale = 1f;

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

    
    // Load scene with fade effect (by name)
    
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
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }

    
    // Load scene with fade effect (by index)
    
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
            UnityEngine.SceneManagement.SceneManager.LoadScene(index);
        }
    }

    void OnDestroy()
    {
        // Make sure game is unpaused if popup is destroyed
        Time.timeScale = 1f;
    }
}