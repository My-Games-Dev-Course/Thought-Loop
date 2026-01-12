using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Shows a simple death message before restarting the level
public class DeathMessage : MonoBehaviour
{
    public static DeathMessage Instance { get; private set; }

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI deathText;
    [SerializeField] private GameObject deathPanel;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 1.5f;

    private bool isShowingMessage = false;

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

        // Hide at start
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
    }

    // Show death message and restart after delay
    public void ShowDeathMessage(string reason)
    {
        if (isShowingMessage) return;

        StartCoroutine(ShowMessageAndRestart(reason));
    }

    private IEnumerator ShowMessageAndRestart(string reason)
    {
        isShowingMessage = true;

        Debug.Log($"[DeathMessage] Player died: {reason}");

        // Freeze everything
        Time.timeScale = 0f;

        // Display message
        if (deathText != null)
        {
            deathText.text = $"{reason}";
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }

        // Wait using unscaled time (works when game is paused)
        yield return new WaitForSecondsRealtime(displayDuration);

        // Hide
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        // Unpause before restarting
        Time.timeScale = 1f;

        // Restart
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);

        isShowingMessage = false;
    }

    // Quick restart without message
    public void RestartImmediately()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}