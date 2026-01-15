using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

// Pause menu with continue, restart level, and restart game
public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button restartLevelButton;
    [SerializeField] private Button restartGameButton;

    [Header("Pause Button (Optional)")]
    [SerializeField] private Button pauseButton; // Button to open pause menu

    [Header("Settings")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private string firstLevelName = "Level1";

    private bool isPaused = false;
    private bool justPaused = false; // Prevent immediate unpause

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

        // Make sure pause panel blocks clicks but doesn't respond to them
        if (pausePanel != null)
        {
            Image panelImage = pausePanel.GetComponent<Image>();
            if (panelImage == null)
            {
                panelImage = pausePanel.AddComponent<Image>();
                panelImage.color = new Color(0, 0, 0, 0.8f); // Dark semi-transparent
            }
            panelImage.raycastTarget = true; // Block clicks through panel

            // Remove any Button component from panel if exists
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

        // Connect pause button if exists
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
        // Toggle pause with escape key
        if (Input.GetKeyDown(pauseKey) && !justPaused)
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

        // Prevent Enter from doing anything while paused
        if (isPaused && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            // Key press handled here to avoid conflicts
        }
    }

    // Open pause menu
    public void Pause()
    {
        if (isPaused)
        {
            Debug.LogWarning("[PauseMenu] Already paused, ignoring");
            return;
        }

        Debug.Log("[PauseMenu] ===== PAUSE BUTTON CLICKED =====");
        Debug.Log("[PauseMenu] Opening pause menu...");

        isPaused = true;
        justPaused = true;
        Time.timeScale = 0f; // Freeze game

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            Debug.Log("[PauseMenu] Pause panel is now ACTIVE");
        }
        else
        {
            Debug.LogError("[PauseMenu] Pause panel is NULL!");
        }

        // Hide pause button when menu is open
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
        }

        // Allow toggle after short delay
        StartCoroutine(ResetJustPaused());
    }

    private IEnumerator ResetJustPaused()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        justPaused = false;
        Debug.Log("[PauseMenu] Can now unpause");
    }

    // Close pause menu and continue
    public void Resume()
    {
        if (!isPaused)
        {
            Debug.LogWarning("[PauseMenu] Not paused, ignoring");
            return;
        }

        Debug.Log("[PauseMenu] Game resumed");

        isPaused = false;
        Time.timeScale = 1f; // Unfreeze game

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Show pause button again
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(true);
        }
    }

    // Restart current level
    public void RestartLevel()
    {
        Debug.Log("[PauseMenu] ===== RESTART LEVEL CLICKED =====");
        Debug.Log("[PauseMenu] Restarting level");

        // Unpause before loading
        Time.timeScale = 1f;
        isPaused = false;

        // Reload current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    // Restart entire game from first level
    public void RestartGame()
    {
        Debug.Log("[PauseMenu] Restarting game");

        // Unpause before loading
        Time.timeScale = 1f;
        isPaused = false;

        // Load first level
        if (!string.IsNullOrEmpty(firstLevelName))
        {
            SceneManager.LoadScene(firstLevelName);
        }
        else
        {
            // Fallback: load scene at build index 0
            SceneManager.LoadScene(0);
        }
    }

    // Check if currently paused
    public bool IsPaused()
    {
        return isPaused;
    }

    void OnDestroy()
    {
        // Make sure game is unpaused when destroyed
        Time.timeScale = 1f;
    }
}