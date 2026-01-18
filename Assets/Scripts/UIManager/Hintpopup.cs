using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * Hint Popup - Shows hint messages and pauses the game
 * Based on VictoryPopup pattern but for displaying hints
 */
public class HintPopup : MonoBehaviour
{
    public static HintPopup Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Keyboard Input")]
    [SerializeField]
    private InputAction closeInputAction = new InputAction(
        type: InputActionType.Button,
        binding: "<Keyboard>/enter"
    );

    private bool isShowing = false;

    void Awake()
    {
        // Always update to current scene instance
        Instance = this;
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        closeInputAction.Enable();
    }

    void OnDisable()
    {
        closeInputAction.Disable();
    }

    void Update()
    {
        // Close on Enter key
        if (isShowing && closeInputAction.WasPressedThisFrame())
        {
            Debug.Log("[HintPopup] Enter pressed - closing");
            CloseHint();
        }
    }

    public void ShowHint(string message)
    {
        if (messageText == null)
        {
            Debug.LogError("[HintPopup] messageText not assigned!");
            return;
        }

        messageText.text = message;
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        isShowing = true;

        Debug.Log($"[HintPopup] Showing: {message}");
    }

    public void CloseHint()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        isShowing = false;

        Debug.Log("[HintPopup] Closed");
    }

    // Public method to check if hint is showing
    public bool IsShowing()
    {
        return isShowing;
    }

    void OnDestroy()
    {
        // Cleanup
        if (Instance == this)
        {
            Instance = null;
        }

        // Make sure game isn't paused
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }

        closeInputAction.Disable();
    }
}