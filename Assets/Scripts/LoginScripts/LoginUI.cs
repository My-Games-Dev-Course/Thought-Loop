//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using UnityEngine.SceneManagement;

///**
// * Login UI - Calls separate Register and Login methods
// * Based on lesson code pattern
// */
//public class LoginUI : MonoBehaviour
//{
//    [Header("UI References")]
//    [SerializeField] private TMP_InputField usernameInput;
//    [SerializeField] private TMP_InputField passwordInput;
//    [SerializeField] private Button registerButton;
//    [SerializeField] private Button loginButton;
//    [SerializeField] private TextMeshProUGUI statusText;

//    [Header("Settings")]
//    [SerializeField] private int firstLevelBuildIndex = 1;

//    private void Start()
//    {
//        Debug.Log("[LoginUI] Starting setup...");

//        // Setup Register button
//        if (registerButton != null)
//        {
//            registerButton.onClick.AddListener(OnRegisterButtonClicked);
//            Debug.Log("[LoginUI] Register button connected");
//        }

//        // Setup Login button
//        if (loginButton != null)
//        {
//            loginButton.onClick.AddListener(OnLoginButtonClicked);
//            Debug.Log("[LoginUI] Login button connected");
//        }

//        // Setup password field (auto-filled and disabled)
//        if (passwordInput != null)
//        {
//            passwordInput.text = "********";
//            passwordInput.interactable = false;
//            passwordInput.contentType = TMP_InputField.ContentType.Password;
//            Debug.Log("[LoginUI] Password field setup");
//        }

//        // Initial status
//        if (statusText != null)
//        {
//            ShowStatus("Enter username - Register for new, Login for existing", false);
//        }

//        // Check CloudSaveManager
//        if (CloudSaveManager.Instance != null)
//        {
//            Debug.Log("[LoginUI] ✓ CloudSaveManager found");
//        }
//        else
//        {
//            Debug.LogError("[LoginUI] ✗ CloudSaveManager NOT FOUND!");
//        }

//        // Allow Enter key
//        if (usernameInput != null)
//        {
//            usernameInput.onSubmit.AddListener(delegate { OnLoginButtonClicked(); });
//        }

//        Debug.Log("[LoginUI] Setup complete");
//    }

//    /**
//     * REGISTER button - Create NEW account
//     */
//    private async void OnRegisterButtonClicked()
//    {
//        Debug.Log("========================================");
//        Debug.Log("[LoginUI] REGISTER BUTTON CLICKED");
//        Debug.Log("========================================");

//        if (CloudSaveManager.Instance == null)
//        {
//            ShowStatus("ERROR: CloudSaveManager not found", true);
//            return;
//        }

//        string username = usernameInput != null ? usernameInput.text.Trim() : "";
//        Debug.Log($"[LoginUI] Username: '{username}'");

//        if (!ValidateUsername(username))
//        {
//            return;
//        }

//        SetButtonsEnabled(false);
//        ShowStatus("Creating account...", false);

//        // Call REGISTER method (creates new account)
//        string result = await CloudSaveManager.Instance.RegisterWithUsername(username);

//        Debug.Log($"[LoginUI] Register result: {result}");

//        if (result.Contains("successful"))
//        {
//            Debug.Log("[LoginUI] ✓✓✓ REGISTER SUCCESS ✓✓✓");
//            ShowStatus("Account created! Loading game...", false);
//            await LoadGameAfterLogin();
//        }
//        else
//        {
//            Debug.LogError("[LoginUI] ✗✗✗ REGISTER FAILED ✗✗✗");
//            ShowStatus($"Registration failed: {result}", true);
//            SetButtonsEnabled(true);
//        }
//    }

//    /**
//     * LOGIN button - Sign into EXISTING account
//     */
//    private async void OnLoginButtonClicked()
//    {
//        Debug.Log("========================================");
//        Debug.Log("[LoginUI] LOGIN BUTTON CLICKED");
//        Debug.Log("========================================");

//        if (CloudSaveManager.Instance == null)
//        {
//            ShowStatus("ERROR: CloudSaveManager not found", true);
//            return;
//        }

//        string username = usernameInput != null ? usernameInput.text.Trim() : "";
//        Debug.Log($"[LoginUI] Username: '{username}'");

//        if (!ValidateUsername(username))
//        {
//            return;
//        }

//        SetButtonsEnabled(false);
//        ShowStatus("Signing in...", false);

//        // Call LOGIN method (signs into existing account)
//        string result = await CloudSaveManager.Instance.LoginWithUsername(username);

//        Debug.Log($"[LoginUI] Login result: {result}");

//        if (result.Contains("successful"))
//        {
//            Debug.Log("[LoginUI] ✓✓✓ LOGIN SUCCESS ✓✓✓");
//            ShowStatus("Login successful! Loading game...", false);
//            await LoadGameAfterLogin();
//        }
//        else
//        {
//            Debug.LogError("[LoginUI] ✗✗✗ LOGIN FAILED ✗✗✗");
//            ShowStatus($"Login failed: {result}", true);
//            SetButtonsEnabled(true);
//        }
//    }

//    /**
//     * Validate username
//     */
//    private bool ValidateUsername(string username)
//    {
//        if (string.IsNullOrEmpty(username))
//        {
//            ShowStatus("Please enter a username", true);
//            return false;
//        }

//        if (username.Length < 3)
//        {
//            ShowStatus("Username must be at least 3 characters", true);
//            return false;
//        }

//        if (username.Length > 20)
//        {
//            ShowStatus("Username must be 20 characters or less", true);
//            return false;
//        }

//        if (!System.Text.RegularExpressions.Regex.IsMatch(username, "^[a-zA-Z0-9]+$"))
//        {
//            ShowStatus("Username: letters and numbers only", true);
//            return false;
//        }

//        return true;
//    }

//    /**
//     * Load game after successful login
//     * Always loads MainMenu - the Play button there will check for saved progress
//     */
//    private async System.Threading.Tasks.Task LoadGameAfterLogin()
//    {
//        Debug.Log("[LoginUI] Loading game...");

//        // Load saved level from cloud and store in PlayerPrefs
//        int savedLevelIndex = await CloudSaveManager.Instance.LoadCurrentLevel();
//        Debug.Log($"[LoginUI] Saved level index from cloud: {savedLevelIndex}");

//        // Save to PlayerPrefs so StartGameButton can read it
//        PlayerPrefs.SetInt("CurrentLevelIndex", savedLevelIndex);
//        PlayerPrefs.Save();
//        Debug.Log($"[LoginUI] Saved to PlayerPrefs: CurrentLevelIndex = {savedLevelIndex}");

//        // ALWAYS load MainMenu (index 1)
//        // The Play button there will check PlayerPrefs and load saved level if exists
//        int mainMenuIndex = firstLevelBuildIndex; // Should be 1 (MainMenu)

//        // Get scene name
//        string sceneName = "MainMenu";
//        if (mainMenuIndex < SceneManager.sceneCountInBuildSettings)
//        {
//            sceneName = System.IO.Path.GetFileNameWithoutExtension(
//                SceneUtility.GetScenePathByBuildIndex(mainMenuIndex)
//            );
//        }

//        Debug.Log($"[LoginUI] Loading MainMenu (index {mainMenuIndex})");
//        ShowStatus($"Loading {sceneName}...", false);

//        await System.Threading.Tasks.Task.Delay(500);

//        if (mainMenuIndex >= 0 && mainMenuIndex < SceneManager.sceneCountInBuildSettings)
//        {
//            SceneManager.LoadScene(mainMenuIndex);
//        }
//        else
//        {
//            Debug.LogError($"[LoginUI] Invalid scene index: {mainMenuIndex}");
//            ShowStatus("ERROR: Invalid level index", true);
//            SetButtonsEnabled(true);
//        }
//    }

//    /**
//     * Show status message
//     */
//    private void ShowStatus(string message, bool isError)
//    {
//        if (statusText != null)
//        {
//            statusText.text = message;
//            statusText.color = isError ? Color.red : Color.white;
//        }
//        Debug.Log($"[LoginUI] Status: {message}");
//    }

//    /**
//     * Enable/disable buttons
//     */
//    private void SetButtonsEnabled(bool enabled)
//    {
//        if (registerButton != null)
//        {
//            registerButton.interactable = enabled;
//        }
//        if (loginButton != null)
//        {
//            loginButton.interactable = enabled;
//        }
//    }
//}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/**
 * Login UI - Works with PlayerData system
 * Saves player name and loads their saved progress
 */
public class LoginUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button loginButton;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Settings")]
    [SerializeField] private int mainMenuIndex = 1; // MainMenu build index

    private void Start()
    {
        Debug.Log("[LoginUI] Starting setup...");

        // Setup Register button
        if (registerButton != null)
        {
            registerButton.onClick.AddListener(OnRegisterButtonClicked);
        }

        // Setup Login button
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }

        // Setup password field
        if (passwordInput != null)
        {
            passwordInput.text = "********";
            passwordInput.interactable = false;
            passwordInput.contentType = TMP_InputField.ContentType.Password;
        }

        // Initial status
        if (statusText != null)
        {
            ShowStatus("Enter username - Register for new, Login for existing", false);
        }

        // Allow Enter key
        if (usernameInput != null)
        {
            usernameInput.onSubmit.AddListener(delegate { OnLoginButtonClicked(); });
        }

        Debug.Log("[LoginUI] Setup complete");
    }

    /**
     * REGISTER button
     */
    private async void OnRegisterButtonClicked()
    {
        Debug.Log("[LoginUI] REGISTER clicked");

        string username = usernameInput != null ? usernameInput.text.Trim() : "";

        if (!ValidateUsername(username))
        {
            return;
        }

        SetButtonsEnabled(false);
        ShowStatus("Creating account...", false);

        string result = await CloudSaveManager.Instance.RegisterWithUsername(username);

        if (result.Contains("successful"))
        {
            Debug.Log("[LoginUI] ✓ Register success");
            ShowStatus("Account created! Loading game...", false);

            // New player always starts at level 2 (Tutorial1)
            int savedLevel = CloudSaveManager.Instance.GetCurrentLevel();
            Debug.Log($"[LoginUI] New player's starting level: {savedLevel}");

            // Save to PlayerPrefs
            PlayerPrefs.SetInt("CurrentLevelIndex", savedLevel);
            PlayerPrefs.Save();

            await System.Threading.Tasks.Task.Delay(500);

            // New players always go to MainMenu (level 2 = Tutorial1)
            LoadMainMenu();
        }
        else
        {
            Debug.LogError($"[LoginUI] ✗ Register failed: {result}");
            ShowStatus($"Registration failed: {result}", true);
            SetButtonsEnabled(true);
        }
    }

    /**
     * LOGIN button
     */
    private async void OnLoginButtonClicked()
    {
        Debug.Log("[LoginUI] LOGIN clicked");

        string username = usernameInput != null ? usernameInput.text.Trim() : "";

        if (!ValidateUsername(username))
        {
            return;
        }

        SetButtonsEnabled(false);
        ShowStatus("Signing in...", false);

        string result = await CloudSaveManager.Instance.LoginWithUsername(username);

        if (result.Contains("successful"))
        {
            Debug.Log("[LoginUI] ✓ Login success");
            ShowStatus("Login successful! Loading game...", false);

            // Get saved level from PlayerData
            int savedLevel = CloudSaveManager.Instance.GetCurrentLevel();
            Debug.Log($"[LoginUI] Player's saved level: {savedLevel}");

            // Save to PlayerPrefs for StartGameButton
            PlayerPrefs.SetInt("CurrentLevelIndex", savedLevel);
            PlayerPrefs.Save();

            await System.Threading.Tasks.Task.Delay(500);

            // Load the appropriate scene based on saved progress
            LoadGameBasedOnProgress(savedLevel);
        }
        else
        {
            Debug.LogError($"[LoginUI] ✗ Login failed: {result}");
            ShowStatus($"Login failed: {result}", true);
            SetButtonsEnabled(true);
        }
    }

    /**
     * Load MainMenu
     */
    private void LoadMainMenu()
    {
        Debug.Log($"[LoginUI] Loading MainMenu (index {mainMenuIndex})");
        SceneManager.LoadScene(mainMenuIndex);
    }

    /**
     * Load game based on player's progress
     * New players (level 2) → MainMenu
     * Returning players (level > 2) → Load saved level directly
     */
    private void LoadGameBasedOnProgress(int savedLevel)
    {
        // Check if this is a new player or returning player
        if (savedLevel <= 2)
        {
            // New player → Go to MainMenu
            Debug.Log($"[LoginUI] New player detected (level {savedLevel}) → Loading MainMenu");
            LoadMainMenu();
        }
        else
        {
            // Returning player → Skip MainMenu, load saved level directly
            Debug.Log($"[LoginUI] Returning player detected (level {savedLevel}) → Loading saved level directly!");

            string sceneName = "Level";
            if (savedLevel < SceneManager.sceneCountInBuildSettings)
            {
                sceneName = System.IO.Path.GetFileNameWithoutExtension(
                    SceneUtility.GetScenePathByBuildIndex(savedLevel)
                );
            }

            Debug.Log($"[LoginUI] Loading scene: {sceneName} (index {savedLevel})");
            SceneManager.LoadScene(savedLevel);
        }
    }

    /**
     * Validate username
     */
    private bool ValidateUsername(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            ShowStatus("Please enter a username", true);
            return false;
        }

        if (username.Length < 3)
        {
            ShowStatus("Username must be at least 3 characters", true);
            return false;
        }

        if (username.Length > 20)
        {
            ShowStatus("Username must be 20 characters or less", true);
            return false;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(username, "^[a-zA-Z0-9]+$"))
        {
            ShowStatus("Username: letters and numbers only", true);
            return false;
        }

        return true;
    }

    /**
     * Show status message
     */
    private void ShowStatus(string message, bool isError)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = isError ? Color.red : Color.white;
        }
    }

    /**
     * Enable/disable buttons
     */
    private void SetButtonsEnabled(bool enabled)
    {
        if (registerButton != null)
        {
            registerButton.interactable = enabled;
        }
        if (loginButton != null)
        {
            loginButton.interactable = enabled;
        }
    }
}