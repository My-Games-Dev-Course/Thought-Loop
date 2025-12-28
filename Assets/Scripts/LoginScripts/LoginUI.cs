//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using UnityEngine.SceneManagement;

///**
// * Login UI - Works with PlayerData system
// * Saves player name and loads their saved progress
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
//    [SerializeField] private int mainMenuIndex = 1; // MainMenu build index

//    private void Start()
//    {
//        Debug.Log("[LoginUI] Starting setup...");

//        // Setup Register button
//        if (registerButton != null)
//        {
//            registerButton.onClick.AddListener(OnRegisterButtonClicked);
//        }

//        // Setup Login button
//        if (loginButton != null)
//        {
//            loginButton.onClick.AddListener(OnLoginButtonClicked);
//        }

//        // Setup password field
//        if (passwordInput != null)
//        {
//            passwordInput.text = "********";
//            passwordInput.interactable = false;
//            passwordInput.contentType = TMP_InputField.ContentType.Password;
//        }

//        // Initial status
//        if (statusText != null)
//        {
//            ShowStatus("Enter username - Register for new, Login for existing", false);
//        }

//        // Allow Enter key
//        if (usernameInput != null)
//        {
//            usernameInput.onSubmit.AddListener(delegate { OnLoginButtonClicked(); });
//        }

//        Debug.Log("[LoginUI] Setup complete");
//    }

//    /**
//     * REGISTER button
//     */
//    private async void OnRegisterButtonClicked()
//    {
//        Debug.Log("[LoginUI] REGISTER clicked");

//        string username = usernameInput != null ? usernameInput.text.Trim() : "";

//        if (!ValidateUsername(username))
//        {
//            return;
//        }

//        SetButtonsEnabled(false);
//        ShowStatus("Creating account...", false);

//        string result = await CloudSaveManager.Instance.RegisterWithUsername(username);

//        if (result.Contains("successful"))
//        {
//            Debug.Log("[LoginUI] ✓ Register success");
//            ShowStatus("Account created! Loading game...", false);

//            // New player always starts at level 2 (Tutorial1)
//            int savedLevel = CloudSaveManager.Instance.GetCurrentLevel();
//            Debug.Log($"[LoginUI] New player's starting level: {savedLevel}");

//            // Save to PlayerPrefs
//            PlayerPrefs.SetInt("CurrentLevelIndex", savedLevel);
//            PlayerPrefs.Save();

//            await System.Threading.Tasks.Task.Delay(500);

//            // New players always go to MainMenu (level 2 = Tutorial1)
//            LoadMainMenu();
//        }
//        else
//        {
//            Debug.LogError($"[LoginUI] ✗ Register failed: {result}");
//            ShowStatus($"Registration failed: {result}", true);
//            SetButtonsEnabled(true);
//        }
//    }

//    /**
//     * LOGIN button
//     */
//    private async void OnLoginButtonClicked()
//    {
//        Debug.Log("[LoginUI] LOGIN clicked");

//        string username = usernameInput != null ? usernameInput.text.Trim() : "";

//        if (!ValidateUsername(username))
//        {
//            return;
//        }

//        SetButtonsEnabled(false);
//        ShowStatus("Signing in...", false);

//        string result = await CloudSaveManager.Instance.LoginWithUsername(username);

//        if (result.Contains("successful"))
//        {
//            Debug.Log("[LoginUI] ✓ Login success");
//            ShowStatus("Login successful! Loading game...", false);

//            // Get saved level from PlayerData
//            int savedLevel = CloudSaveManager.Instance.GetCurrentLevel();
//            Debug.Log($"[LoginUI] Player's saved level: {savedLevel}");

//            // Save to PlayerPrefs for StartGameButton
//            PlayerPrefs.SetInt("CurrentLevelIndex", savedLevel);
//            PlayerPrefs.Save();

//            await System.Threading.Tasks.Task.Delay(500);

//            // Load the appropriate scene based on saved progress
//            LoadGameBasedOnProgress(savedLevel);
//        }
//        else
//        {
//            Debug.LogError($"[LoginUI] ✗ Login failed: {result}");
//            ShowStatus($"Login failed: {result}", true);
//            SetButtonsEnabled(true);
//        }
//    }

//    /**
//     * Load MainMenu
//     */
//    private void LoadMainMenu()
//    {
//        Debug.Log($"[LoginUI] Loading MainMenu (index {mainMenuIndex})");
//        SceneManager.LoadScene(mainMenuIndex);
//    }

//    /**
//     * Load game based on player's progress
//     * New players (level 2) → MainMenu
//     * Returning players (level > 2) → Load saved level directly
//     */
//    //private void LoadGameBasedOnProgress(int savedLevel)
//    //{
//    //    // Check if this is a new player or returning player
//    //    if (savedLevel <= 2)
//    //    {
//    //        // New player → Go to MainMenu
//    //        Debug.Log($"[LoginUI] New player detected (level {savedLevel}) → Loading MainMenu");
//    //        LoadMainMenu();
//    //    }
//    //    else
//    //    {
//    //        // Returning player → Skip MainMenu, load saved level directly
//    //        Debug.Log($"[LoginUI] Returning player detected (level {savedLevel}) → Loading saved level directly!");

//    //        string sceneName = "Level";
//    //        if (savedLevel < SceneManager.sceneCountInBuildSettings)
//    //        {
//    //            sceneName = System.IO.Path.GetFileNameWithoutExtension(
//    //                SceneUtility.GetScenePathByBuildIndex(savedLevel)
//    //            );
//    //        }

//    //        Debug.Log($"[LoginUI] Loading scene: {sceneName} (index {savedLevel})");
//    //        SceneManager.LoadScene(savedLevel);
//    //    }
//    //}

//    private void LoadGameBasedOnProgress(int savedLevel)
//    {
//        // Check if valid level
//        if (savedLevel < 0 || savedLevel >= SceneManager.sceneCountInBuildSettings)
//        {
//            Debug.LogWarning($"[LoginUI] Invalid level {savedLevel}, loading MainMenu");
//            LoadMainMenu();
//            return;
//        }

//        // New player (level 2 = Tutorial1) → MainMenu
//        if (savedLevel <= 2)
//        {
//            Debug.Log($"[LoginUI] New player (level {savedLevel}) → MainMenu");
//            LoadMainMenu();
//        }
//        else
//        {
//            // Returning player → Skip MainMenu, load saved level directly!
//            Debug.Log($"[LoginUI] Returning player (level {savedLevel}) → Loading directly!");
//            SceneManager.LoadScene(savedLevel);
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
//     * Show status message
//     */
//    private void ShowStatus(string message, bool isError)
//    {
//        if (statusText != null)
//        {
//            statusText.text = message;
//            statusText.color = isError ? Color.red : Color.white;
//        }
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
using System.Collections;

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

    private bool isBusy = false;

    private void Start()
    {
        Debug.Log("[LoginUI] Starting setup...");

        if (registerButton != null)
            registerButton.onClick.AddListener(OnRegisterClicked);

        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginClicked);

        if (passwordInput != null)
        {
            passwordInput.text = "********";
            passwordInput.interactable = false;
            passwordInput.contentType = TMP_InputField.ContentType.Password;
        }

        if (usernameInput != null)
            usernameInput.onSubmit.AddListener(_ => OnLoginClicked());

        ShowStatus("Enter username - Register for new, Login for existing", false);
        Debug.Log("[LoginUI] Setup complete");
    }

    // =========================
    // BUTTON HANDLERS
    // =========================

    private void OnLoginClicked()
    {
        if (isBusy) return;
        StartCoroutine(LoginFlow());
    }

    private void OnRegisterClicked()
    {
        if (isBusy) return;
        StartCoroutine(RegisterFlow());
    }

    // =========================
    // LOGIN FLOW
    // =========================

    private IEnumerator LoginFlow()
    {
        isBusy = true;
        SetButtonsEnabled(false);

        string username = usernameInput.text.Trim();
        if (!ValidateUsername(username))
        {
            ResetUI();
            yield break;
        }

        ShowStatus("Signing in...", false);
        Debug.Log("[LoginUI] LOGIN clicked");

        var loginTask = CloudSaveManager.Instance.LoginWithUsername(username);
        yield return new WaitUntil(() => loginTask.IsCompleted);

        string result = loginTask.Result;

        if (!result.Contains("successful"))
        {
            Debug.LogError($"[LoginUI] Login failed: {result}");
            ShowStatus($"Login failed: {result}", true);
            ResetUI();
            yield break;
        }

        Debug.Log("[LoginUI] ✓ Login success");

        int savedLevel = CloudSaveManager.Instance.GetCurrentLevel();
        Debug.Log($"[LoginUI] Player's saved level: {savedLevel}");

        PlayerPrefs.SetInt("CurrentLevelIndex", savedLevel);
        PlayerPrefs.Save();

        ShowStatus("Loading game...", false);
        yield return new WaitForSeconds(0.3f);

        yield return LoadGameByProgress(savedLevel);
    }

    // =========================
    // REGISTER FLOW
    // =========================

    private IEnumerator RegisterFlow()
    {
        isBusy = true;
        SetButtonsEnabled(false);

        string username = usernameInput.text.Trim();
        if (!ValidateUsername(username))
        {
            ResetUI();
            yield break;
        }

        ShowStatus("Creating account...", false);
        Debug.Log("[LoginUI] REGISTER clicked");

        var registerTask = CloudSaveManager.Instance.RegisterWithUsername(username);
        yield return new WaitUntil(() => registerTask.IsCompleted);

        string result = registerTask.Result;

        if (!result.Contains("successful"))
        {
            Debug.LogError($"[LoginUI] Register failed: {result}");
            ShowStatus($"Registration failed: {result}", true);
            ResetUI();
            yield break;
        }

        Debug.Log("[LoginUI] ✓ Register success");

        int startLevel = CloudSaveManager.Instance.GetCurrentLevel();
        PlayerPrefs.SetInt("CurrentLevelIndex", startLevel);
        PlayerPrefs.Save();

        ShowStatus("Account created! Loading game...", false);
        yield return new WaitForSeconds(0.3f);

        yield return LoadGameByProgress(startLevel);
    }

    // =========================
    // SCENE LOADING (WEBGL SAFE)
    // =========================

    private IEnumerator LoadGameByProgress(int savedLevel)
    {
        int sceneIndex = mainMenuIndex;

        if (savedLevel > 2 &&
            savedLevel >= 0 &&
            savedLevel < SceneManager.sceneCountInBuildSettings)
        {
            sceneIndex = savedLevel;
        }

        Debug.Log($"[LoginUI] Loading scene index {sceneIndex}");

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        while (!op.isDone)
            yield return null;
    }

    // =========================
    // HELPERS
    // =========================

    private void ResetUI()
    {
        isBusy = false;
        SetButtonsEnabled(true);
    }

    private void SetButtonsEnabled(bool enabled)
    {
        if (loginButton != null) loginButton.interactable = enabled;
        if (registerButton != null) registerButton.interactable = enabled;
    }

    private void ShowStatus(string message, bool isError)
    {
        if (statusText == null) return;
        statusText.text = message;
        statusText.color = isError ? Color.red : Color.white;
    }

    private bool ValidateUsername(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            ShowStatus("Please enter a username", true);
            return false;
        }

        if (username.Length < 3 || username.Length > 20)
        {
            ShowStatus("Username must be 3–20 characters", true);
            return false;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(username, "^[a-zA-Z0-9]+$"))
        {
            ShowStatus("Letters and numbers only", true);
            return false;
        }

        return true;
    }
}
