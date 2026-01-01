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
    [SerializeField] private Button guestButton; // Won't save data
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

        if (guestButton != null)
            guestButton.onClick.AddListener(OnGuestClicked);

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
        if (guestButton != null) guestButton.interactable = enabled;
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


    // =========================
    // GUEST MODE
    // =========================

    private void OnGuestClicked()
    {
        if (isBusy) return;
        StartCoroutine(GuestFlow());
    }

    private IEnumerator GuestFlow()
    {
        isBusy = true;
        SetButtonsEnabled(false);

        Debug.Log("[LoginUI] GUEST MODE - Starting without login");
        ShowStatus("Starting game as guest...", false);

        // Set guest mode flag so CloudSaveManager knows not to save
        PlayerPrefs.SetInt("IsGuestMode", 1);
        PlayerPrefs.SetInt("CurrentLevelIndex", 2); // Start at Tutorial1
        PlayerPrefs.Save();

        yield return new WaitForSeconds(0.3f);

        // Load main menu (or first level)
        Debug.Log($"[LoginUI] Loading MainMenu (index {mainMenuIndex}) as guest");

        AsyncOperation op = SceneManager.LoadSceneAsync(mainMenuIndex);
        while (!op.isDone)
            yield return null;
    }

}
