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
            // Load by build index
            if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                Debug.LogError($"[StartGameButton] Invalid scene index: {sceneIndex}");
            }
        }
        else
        {
            // Load by scene name
            if (!string.IsNullOrEmpty(firstSceneName))
            {
                SceneManager.LoadScene(firstSceneName);
            }
            else
            {
                Debug.LogError("[StartGameButton] Scene name is empty!");
            }
        }
    }
}