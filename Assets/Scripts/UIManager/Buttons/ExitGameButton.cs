using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitGameButton : MonoBehaviour
{
    [Header("Scene to Load")]
    [Tooltip("Name of the first scene (e.g., 'Main Menu')")]
    [SerializeField] private string firstSceneName = "Main Menu";

    [Header("Alternative: Use Build Index")]
    [Tooltip("If you prefer to use build index instead of name")]
    [SerializeField] private bool useBuildIndex = false;
    [SerializeField] private int sceneIndex = 0;

    private Button button;

    void Start()
    {
        // Get the Button component
        button = GetComponent<Button>();

        if (button != null)
        {
            // Add listener to the button
            button.onClick.AddListener(OnExitButtonClicked);
            Debug.Log("[ExitButton] Exit button listener added");
        }
        else
        {
            Debug.LogError("[ExitButton] No Button component found!");
        }
    }

    void OnExitButtonClicked()
    {
        Debug.Log("[ExitButton] Exit button clicked - returning to first scene...");
        ExitToFirstScene();
    }

    // Call this method from the Button's OnClick event (alternative to automatic setup)
    public void ExitToFirstScene()
    {
        if (useBuildIndex)
        {
            // Load by build index WITH FADE
            if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                LoadSceneWithFade(sceneIndex);
            }
            else
            {
                Debug.LogError($"[ExitButton] Invalid scene index: {sceneIndex}");
            }
        }
        else
        {
            // Load by scene name WITH FADE
            if (!string.IsNullOrEmpty(firstSceneName))
            {
                LoadSceneWithFade(firstSceneName);
            }
            else
            {
                Debug.LogError("[ExitButton] Scene name is empty!");
            }
        }
    }

    // Load scene with fade effect (by name)
    // Falls back to direct load if SceneFader doesn't exist
    private void LoadSceneWithFade(string sceneName)
    {
        if (SceneFader.Instance != null)
        {
            // Use fade transition
            SceneFader.Instance.FadeToScene(sceneName);
            Debug.Log($"[ExitButton] Loading scene '{sceneName}' with fade effect");
        }
        else
        {
            // Fallback: direct load without fade
            Debug.LogWarning("[ExitButton] SceneFader not found! Loading scene without fade effect.");
            SceneManager.LoadScene(sceneName);
        }
    }

    // Load scene with fade effect (by index)
    // Falls back to direct load if SceneFader doesn't exist
    private void LoadSceneWithFade(int index)
    {
        if (SceneFader.Instance != null)
        {
            // Use fade transition
            SceneFader.Instance.FadeToScene(index);
            Debug.Log($"[ExitButton] Loading scene index {index} with fade effect");
        }
        else
        {
            // Fallback: direct load without fade
            Debug.LogWarning("[ExitButton] SceneFader not found! Loading scene without fade effect.");
            SceneManager.LoadScene(index);
        }
    }

    void OnDestroy()
    {
        // Clean up the listener
        if (button != null)
        {
            button.onClick.RemoveListener(OnExitButtonClicked);
        }
    }
}