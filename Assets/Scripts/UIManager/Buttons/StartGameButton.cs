using UnityEngine;
using UnityEngine.SceneManagement;
public class StartGameButton : MonoBehaviour
{
    [Header("Scene to Load")]
    [Tooltip("Name of the first level/tutorial scene")]
    [SerializeField] private string firstSceneName = "Tutorial1";

    [Header("Alternative: Use Build Index")]
    [Tooltip("If you prefer to use build index instead of name")]
    [SerializeField] private bool useBuildIndex = false;
    [SerializeField] private int sceneIndex = 1;

    // Call this method from the Button's OnClick event
    public void StartGame()
    {
        Debug.Log("[StartGameButton] Starting game...");

        if (useBuildIndex)
        {
            // Load by build index WITH FADE
            if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                LoadSceneWithFade(sceneIndex);
            }
            else
            {
                Debug.LogError($"[StartGameButton] Invalid scene index: {sceneIndex}");
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
                Debug.LogError("[StartGameButton] Scene name is empty!");
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
            Debug.Log($"[StartGameButton] Loading scene '{sceneName}' with fade effect");
        }
        else
        {
            // Fallback: direct load without fade
            Debug.LogWarning("[StartGameButton] SceneFader not found! Loading scene without fade effect.");
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
            Debug.Log($"[StartGameButton] Loading scene index {index} with fade effect");
        }
        else
        {
            // Fallback: direct load without fade
            Debug.LogWarning("[StartGameButton] SceneFader not found! Loading scene without fade effect.");
            SceneManager.LoadScene(index);
        }
    }
}
