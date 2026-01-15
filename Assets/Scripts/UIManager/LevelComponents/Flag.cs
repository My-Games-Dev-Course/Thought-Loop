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

    [Header("Victory Popup")]
    [Tooltip("Show victory popup before loading next scene?")]
    [SerializeField] private bool useVictoryPopup = true;

    // For tag detection
    [Header("Settings")]
    [Tooltip("Tag to detect (usually 'Player')")]
    [SerializeField] private string targetTag = "Player";


    public enum SceneLoadMode
    {
        ByIndex,      // Use the scene index
        ByName,       // Use the scene name
        NextInBuild   // Automatically load next scene in build settings
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            if (useVictoryPopup)
            {
                ShowVictoryPopup();
            }
            else
            {
                //LoadNextScene();
            }
        }
    }

    // Show the victory popup (game will pause)
    private void ShowVictoryPopup()
    {
        if (VictoryPopup.Instance == null)
        {
            Debug.LogError("[Flag] VictoryPopup not found! Loading scene directly instead.");
            //LoadNextScene();
            return;
        }

        switch (loadMode)
        {
            case SceneLoadMode.ByIndex:
                Debug.Log($"[Flag] Showing victory popup for scene index: {nextSceneIndex}");
                VictoryPopup.Instance.ShowVictory(sceneIndex: nextSceneIndex);
                break;

            case SceneLoadMode.ByName:
                if (!string.IsNullOrEmpty(nextSceneName))
                {
                    Debug.Log($"[Flag] Showing victory popup for scene: {nextSceneName}");
                    VictoryPopup.Instance.ShowVictory(sceneName: nextSceneName);
                }
                else
                {
                    Debug.LogError("[Flag] Scene name is empty!");
                }
                break;

            case SceneLoadMode.NextInBuild:
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                int nextScene = currentSceneIndex + 1;

                if (nextScene < SceneManager.sceneCountInBuildSettings)
                {
                    Debug.Log($"[Flag] Showing victory popup for next scene: {nextScene}");
                    VictoryPopup.Instance.ShowVictory(sceneIndex: nextScene);
                }
                else
                {
                    Debug.LogWarning("[Flag] No next scene in build settings!");
                }
                break;
        }
    }

    // Load next scene directly (no popup)
    //private void LoadNextScene()
    //{
    //    switch (loadMode)
    //    {
    //        case SceneLoadMode.ByIndex:
    //            Debug.Log($"[Flag] Loading scene by index: {nextSceneIndex}");
    //            LoadSceneWithFade(nextSceneIndex);
    //            break;

    //        case SceneLoadMode.ByName:
    //            if (!string.IsNullOrEmpty(nextSceneName))
    //            {
    //                Debug.Log($"[Flag] Loading scene by name: {nextSceneName}");
    //                LoadSceneWithFade(nextSceneName);
    //            }
    //            else
    //            {
    //                Debug.LogError("[Flag] Scene name is empty!");
    //            }
    //            break;

    //        case SceneLoadMode.NextInBuild:
    //            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    //            int nextScene = currentSceneIndex + 1;

    //            if (nextScene < SceneManager.sceneCountInBuildSettings) 
    //            {
    //                Debug.Log($"[Flag] Loading next scene in build order: {nextScene}");
    //                LoadSceneWithFade(nextScene);
    //            }
    //            else
    //            {
    //                Debug.LogWarning("[Flag] No next scene in build settings!");
    //            }
    //            break;
    //    }
    //}

    //// Load scene with fade effect (by name)
    //private void LoadSceneWithFade(string sceneName)
    //{
    //    if (SceneFader.Instance != null)
    //    {
    //        SceneFader.Instance.FadeToScene(sceneName);
    //        Debug.Log($"[Flag] Loading scene '{sceneName}' with fade effect");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[Flag] SceneFader not found! Loading scene without fade effect.");
    //        SceneManager.LoadScene(sceneName);
    //    }
    //}

    //// Load scene with fade effect (by index)
    //private void LoadSceneWithFade(int index)
    //{
    //    if (SceneFader.Instance != null)
    //    {
    //        SceneFader.Instance.FadeToScene(index);
    //        Debug.Log($"[Flag] Loading scene index {index} with fade effect");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[Flag] SceneFader not found! Loading scene without fade effect.");
    //        SceneManager.LoadScene(index);
    //    }
    //}

    // Want to do also fade if it is by next in build, without specifying index or name

}