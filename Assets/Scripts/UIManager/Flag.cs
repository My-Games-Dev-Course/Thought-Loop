using UnityEngine;
using UnityEngine.SceneManagement;

public class Flag : MonoBehaviour
{
    [Header("Scene Loading Options")]
    [Tooltip("How to determine which scene to load")]
    [SerializeField] private SceneLoadMode loadMode = SceneLoadMode.ByIndex;

    [Tooltip("Build index of the next scene (used when mode is ByIndex)")]
    [SerializeField] private int nextSceneIndex = 1;

    [Tooltip("Name of the scene to load (used when mode is ByName)")]
    [SerializeField] private string nextSceneName = "";

    public enum SceneLoadMode
    {
        ByIndex,      // Use the scene index
        ByName,       // Use the scene name
        NextInBuild   // Automatically load next scene in build settings
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        switch (loadMode)
        {
            case SceneLoadMode.ByIndex:
                Debug.Log($"Loading scene by index: {nextSceneIndex}");
                SceneManager.LoadScene(nextSceneIndex);
                break;

            case SceneLoadMode.ByName:
                if (!string.IsNullOrEmpty(nextSceneName))
                {
                    Debug.Log($"Loading scene by name: {nextSceneName}");
                    SceneManager.LoadScene(nextSceneName);
                }
                else
                {
                    Debug.LogError("[Door] Scene name is empty!");
                }
                break;

            case SceneLoadMode.NextInBuild:
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                int nextScene = currentSceneIndex + 1;

                if (nextScene < SceneManager.sceneCountInBuildSettings)
                {
                    Debug.Log($"Loading next scene in build order: {nextScene}");
                    SceneManager.LoadScene(nextScene);
                }
                else
                {
                    Debug.LogWarning("[Door] No next scene in build settings!");
                }
                break;
        }
    }
}