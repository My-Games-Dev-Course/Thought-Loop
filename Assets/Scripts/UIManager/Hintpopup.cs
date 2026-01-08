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

//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.InputSystem;

//public class HintPopup : MonoBehaviour
//{
//    public static HintPopup Instance { get; private set; }

//    [Header("Popup References")]
//    [SerializeField] private GameObject popupPanel;
//    [SerializeField] private TextMeshProUGUI hintText;
//    [SerializeField] private Button closeButton;

//    [Header("Settings")]
//    [SerializeField] private bool pauseGameWhenShown = true;

//    [Header("Keyboard Input")]
//    [SerializeField]
//    private InputAction closeInputAction = new InputAction(
//        type: InputActionType.Button,
//        binding: "<Keyboard>/enter"
//    );

//    private bool isShowing = false;

//    void Awake()
//    {
//        // Singleton pattern
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//            return;
//        }

//        // Hide popup initially
//        if (popupPanel != null)
//        {
//            popupPanel.SetActive(false);
//        }
//    }

//    void OnEnable()
//    {
//        closeInputAction.Enable();
//    }

//    void OnDisable()
//    {
//        closeInputAction.Disable();
//    }

//    void Start()
//    {
//        // Setup close button if assigned
//        if (closeButton != null)
//        {
//            closeButton.onClick.AddListener(HideHint);
//        }

//        // Add default Enter key binding if none exists
//        if (closeInputAction.bindings.Count == 0)
//        {
//            closeInputAction.AddBinding("<Keyboard>/enter");
//            closeInputAction.AddBinding("<Keyboard>/return");
//        }
//    }

//    void Update()
//    {
//        // Check for Enter key press to CLOSE popup (only when showing)
//        if (isShowing && closeInputAction.WasPressedThisFrame())
//        {
//            Debug.Log("[HintPopup] Enter key pressed - closing popup");
//            HideHint();
//        }
//    }

//    // Show the hint popup with a message
//    public void ShowHint(string message)
//    {
//        if (isShowing) return;

//        Debug.Log("[HintPopup] Showing hint popup");

//        // Set the hint text
//        if (hintText != null)
//        {
//            hintText.text = message;
//        }

//        // Show popup
//        if (popupPanel != null)
//        {
//            popupPanel.SetActive(true);
//        }

//        // Pause game if enabled
//        if (pauseGameWhenShown)
//        {
//            Time.timeScale = 0f;
//            Debug.Log("[HintPopup] Game paused");
//        }

//        isShowing = true;
//    }

//    // Hide the hint popup
//    public void HideHint()
//    {
//        if (!isShowing) return;

//        Debug.Log("[HintPopup] Hiding hint popup");

//        // Hide popup
//        if (popupPanel != null)
//        {
//            popupPanel.SetActive(false);
//        }

//        // Resume game if it was paused
//        if (pauseGameWhenShown)
//        {
//            Time.timeScale = 1f;
//            Debug.Log("[HintPopup] Game resumed");
//        }

//        isShowing = false;
//    }

//    void OnDestroy()
//    {
//        // Make sure game is unpaused
//        if (isShowing && pauseGameWhenShown)
//        {
//            Time.timeScale = 1f;
//        }

//        // Clean up button listener
//        if (closeButton != null)
//        {
//            closeButton.onClick.RemoveListener(HideHint);
//        }
//    }
//}