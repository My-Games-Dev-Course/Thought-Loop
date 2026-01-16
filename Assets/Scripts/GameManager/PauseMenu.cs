using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button restartLevelButton;
    [SerializeField] private Button restartGameButton;

    [Header("Pause Button (Optional)")]
    [SerializeField] private Button pauseButton;

    [Header("Settings")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private string firstLevelName = "Tutorial1";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Recording System (Optional - Auto-finds if not set)")]
    [SerializeField] private RecorderManager recorderManager;
    [SerializeField] private RecorderManagerMult recorderManagerMult;

    private bool isPaused = false;
    private bool justPaused = false;
    private UnityEngine.EventSystems.EventSystem cachedEventSystem;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Auto-find recorder managers if not assigned
        if (recorderManager == null)
        {
            recorderManager = FindObjectOfType<RecorderManager>();
            if (recorderManager != null)
            {
                Debug.Log("[PauseMenu] Auto-found RecorderManager");
            }
        }

        if (recorderManagerMult == null)
        {
            recorderManagerMult = FindObjectOfType<RecorderManagerMult>();
            if (recorderManagerMult != null)
            {
                Debug.Log("[PauseMenu] Auto-found RecorderManagerMult");
            }
        }

        // Make sure pause panel blocks clicks
        if (pausePanel != null)
        {
            Image panelImage = pausePanel.GetComponent<Image>();
            if (panelImage == null)
            {
                panelImage = pausePanel.AddComponent<Image>();
                panelImage.color = new Color(0, 0, 0, 0.8f);
            }
            panelImage.raycastTarget = true;

            Button panelButton = pausePanel.GetComponent<Button>();
            if (panelButton != null)
            {
                Destroy(panelButton);
            }
        }

        // Connect button listeners
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(Resume);
        }

        if (restartLevelButton != null)
        {
            restartLevelButton.onClick.RemoveAllListeners();
            restartLevelButton.onClick.AddListener(RestartLevel);
        }

        if (restartGameButton != null)
        {
            restartGameButton.onClick.RemoveAllListeners();
            restartGameButton.onClick.AddListener(RestartGame);
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(Pause);
        }

        // Start with menu hidden
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    void Update()
    {
        // Don't allow pause if victory popup is showing
        bool victoryIsShowing = VictoryPopup.Instance != null && VictoryPopup.Instance.IsShowing();

        // Toggle pause with escape key (only if victory is not showing)
        if (!victoryIsShowing && Input.GetKeyDown(pauseKey) && !justPaused)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        // Block Enter key completely when paused
        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                // Consume the key to prevent any action
                Input.ResetInputAxes();
                return;
            }
        }
    }

    // Open pause menu
    public void Pause()
    {
        if (isPaused)
        {
            return;
        }

        isPaused = true;
        justPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
        }

        // NEW: Pause recording
        PauseRecording();

        DisableButtonNavigation();
        StartCoroutine(ResetJustPaused());
    }

    private IEnumerator ResetJustPaused()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        justPaused = false;
    }

    // Close pause menu and continue
    public void Resume()
    {
        if (!isPaused)
        {
            return;
        }

        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(true);
        }

        // NEW: Resume recording
        ResumeRecording();

        EnableButtonNavigation();
    }

    // NEW: Pause the recording system
    private void PauseRecording()
    {
        if (recorderManager != null)
        {
            recorderManager.PauseRecording();
        }

        if (recorderManagerMult != null)
        {
            recorderManagerMult.PauseRecording();
        }
    }

    // NEW: Resume the recording system
    private void ResumeRecording()
    {
        if (recorderManager != null)
        {
            recorderManager.ResumeRecording();
        }

        if (recorderManagerMult != null)
        {
            recorderManagerMult.ResumeRecording();
        }
    }

    // Restart current level
    public void RestartLevel()
    {
        // Hide pause panel immediately
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    // Return to main menu
    public void RestartGame()
    {
        Debug.Log("[PauseMenu] RestartGame clicked!");
        Debug.Log($"[PauseMenu] Loading scene: {mainMenuSceneName}");

        // Hide pause panel immediately
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;

        if (SceneFader.Instance != null)
        {
            Debug.Log("[PauseMenu] Using SceneFader");
            SceneFader.Instance.FadeToScene(mainMenuSceneName);
        }
        else
        {
            Debug.Log("[PauseMenu] Loading directly without fade");
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    // Check if currently paused
    public bool IsPaused()
    {
        return isPaused;
    }

    // Disable keyboard navigation on buttons
    private void DisableButtonNavigation()
    {
        cachedEventSystem = UnityEngine.EventSystems.EventSystem.current;

        if (continueButton != null)
        {
            var nav = continueButton.navigation;
            nav.mode = Navigation.Mode.None;
            continueButton.navigation = nav;
        }

        if (restartLevelButton != null)
        {
            var nav = restartLevelButton.navigation;
            nav.mode = Navigation.Mode.None;
            restartLevelButton.navigation = nav;
        }

        if (restartGameButton != null)
        {
            var nav = restartGameButton.navigation;
            nav.mode = Navigation.Mode.None;
            restartGameButton.navigation = nav;
        }

        // Clear selected object to prevent keyboard input
        if (cachedEventSystem != null)
        {
            cachedEventSystem.SetSelectedGameObject(null);
        }
    }

    // Re-enable keyboard navigation
    private void EnableButtonNavigation()
    {
        if (continueButton != null)
        {
            var nav = continueButton.navigation;
            nav.mode = Navigation.Mode.Automatic;
            continueButton.navigation = nav;
        }

        if (restartLevelButton != null)
        {
            var nav = restartLevelButton.navigation;
            nav.mode = Navigation.Mode.Automatic;
            restartLevelButton.navigation = nav;
        }

        if (restartGameButton != null)
        {
            var nav = restartGameButton.navigation;
            nav.mode = Navigation.Mode.Automatic;
            restartGameButton.navigation = nav;
        }
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}