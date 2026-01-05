using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * Hint Popup - Shows hint messages and pauses the game
 * Based on VictoryPopup pattern but for displaying hints
 */
public class HintPopup : MonoBehaviour
{
    public static HintPopup Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI messageText;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false); // Start hidden
    }

    public void ShowHint(string message)
    {
        messageText.text = message;
        gameObject.SetActive(true);
        Time.timeScale = 0f; // Pause
    }

    public void CloseHint()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f; // Resume
    }
}