//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;
//using System.Collections;

//public class SceneFader : MonoBehaviour
//{
//    public static SceneFader Instance { get; private set; }

//    [SerializeField] private Image fadeImage;
//    [SerializeField] private float fadeDuration = 1f;

//    private void Awake()
//    {
//        // Singleton pattern - only one fader exists across scenes
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//            return;
//        }

//        // Make sure we start invisible
//        if (fadeImage != null)
//        {
//            Color c = fadeImage.color;
//            c.a = 0f;
//            fadeImage.color = c;
//        }
//    }

//    // Fade to black, load scene, then fade from black
//    public void FadeToScene(string sceneName)
//    {
//        StartCoroutine(FadeAndLoadScene(sceneName));
//    }

//    // Fade to black, load scene by index, then fade from black
//    public void FadeToScene(int sceneIndex)
//    {
//        StartCoroutine(FadeAndLoadScene(sceneIndex));
//    }

//    private IEnumerator FadeAndLoadScene(string sceneName)
//    {
//        // Fade out (to black)
//        yield return StartCoroutine(FadeOut());

//        // Load the scene
//        SceneManager.LoadScene(sceneName);

//        // Fade in (from black)
//        yield return StartCoroutine(FadeIn());
//    }

//    private IEnumerator FadeAndLoadScene(int sceneIndex)
//    {
//        // Fade out (to black)
//        yield return StartCoroutine(FadeOut());

//        // Load the scene
//        SceneManager.LoadScene(sceneIndex);

//        // Fade in (from black)
//        yield return StartCoroutine(FadeIn());
//    }

//    // Fade out (screen becomes black)
//    private IEnumerator FadeOut()
//    {
//        float elapsedTime = 0f;
//        Color c = fadeImage.color;

//        while (elapsedTime < fadeDuration)
//        {
//            elapsedTime += Time.deltaTime;
//            c.a = Mathf.Clamp01(elapsedTime / fadeDuration);
//            fadeImage.color = c;
//            yield return null;
//        }

//        // Ensure fully opaque
//        c.a = 1f;
//        fadeImage.color = c;
//    }

//    // Fade in (black screen becomes clear)

//    private IEnumerator FadeIn()
//    {
//        float elapsedTime = 0f;
//        Color c = fadeImage.color;

//        while (elapsedTime < fadeDuration)
//        {
//            elapsedTime += Time.deltaTime;
//            c.a = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
//            fadeImage.color = c;
//            yield return null;
//        }

//        // Ensure fully transparent
//        c.a = 0f;
//        fadeImage.color = c;
//    }

//    // Just fade in (useful for scene start)
//    public void FadeInOnly()
//    {
//        StartCoroutine(FadeIn());
//    }

//    // Just fade out (useful for scene end)
//    public void FadeOutOnly()
//    {
//        StartCoroutine(FadeOut());
//    }
//}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage; // Can be left empty, will auto-find
    [SerializeField] private float fadeDuration = 1f;

    [Header("Auto-Find Settings")]
    [SerializeField] private bool autoFindFadePanel = true;
    [SerializeField] private string fadePanelName = "FadePanel";

    private void Awake()
    {
        // Singleton pattern - only one fader exists across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[SceneFader] Instance created and set to DontDestroyOnLoad");
        }
        else
        {
            Debug.Log("[SceneFader] Duplicate instance destroyed");
            Destroy(gameObject);
            return;
        }

        // Auto-find FadePanel if not assigned
        if (fadeImage == null && autoFindFadePanel)
        {
            FindFadePanel();
        }

        // Validate fadeImage
        if (fadeImage == null)
        {
            Debug.LogError("[SceneFader] Fade Image is NOT assigned and could not be found! Please create a UI Image named 'FadePanel' in Canvas!");
            return;
        }

        // Make sure we start invisible
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;
        Debug.Log("[SceneFader] Initialized with alpha = 0");
    }

    // Automatically find FadePanel in the scene
    private void FindFadePanel()
    {
        Debug.Log($"[SceneFader] Searching for FadePanel named '{fadePanelName}'...");

        // Method 1: Find by name
        GameObject fadePanelObj = GameObject.Find(fadePanelName);
        if (fadePanelObj != null)
        {
            fadeImage = fadePanelObj.GetComponent<Image>();
            if (fadeImage != null)
            {
                Debug.Log($"[SceneFader] ✅ Found FadePanel by name: {fadePanelName}");
                return;
            }
        }

        // Method 2: Find in Canvas
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas canvas in canvases)
        {
            Transform fadePanelTransform = canvas.transform.Find(fadePanelName);
            if (fadePanelTransform != null)
            {
                fadeImage = fadePanelTransform.GetComponent<Image>();
                if (fadeImage != null)
                {
                    Debug.Log($"[SceneFader] Found FadePanel in Canvas: {canvas.name}");
                    return;
                }
            }
        }

        Debug.LogWarning($"[SceneFader] Could not find FadePanel named '{fadePanelName}'. Make sure it exists in Canvas!");
    }

    // Re-find FadePanel after scene load (called automatically)
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-find FadePanel in new scene
        if (fadeImage == null && autoFindFadePanel)
        {
            Debug.Log($"[SceneFader] Scene '{scene.name}' loaded, searching for FadePanel...");
            FindFadePanel();
        }
    }

    // Fade to black, load scene, then fade from black
    public void FadeToScene(string sceneName)
    {
        if (!ValidateFadeImage()) return;
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    // Fade to black, load scene by index, then fade from black
    public void FadeToScene(int sceneIndex)
    {
        if (!ValidateFadeImage()) return;
        StartCoroutine(FadeAndLoadScene(sceneIndex));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        Debug.Log($"[SceneFader] Starting fade to scene: {sceneName}");

        // Fade out (to black)
        yield return StartCoroutine(FadeOut());

        // Load the scene
        SceneManager.LoadScene(sceneName);

        // Wait one frame for scene to load
        yield return null;

        // Re-find FadePanel in new scene
        if (fadeImage == null)
        {
            FindFadePanel();
        }

        // Fade in (from black)
        if (fadeImage != null)
        {
            yield return StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeAndLoadScene(int sceneIndex)
    {
        Debug.Log($"[SceneFader] Starting fade to scene index: {sceneIndex}");

        // Fade out (to black)
        yield return StartCoroutine(FadeOut());

        // Load the scene
        SceneManager.LoadScene(sceneIndex);

        // Wait one frame for scene to load
        yield return null;

        // Re-find FadePanel in new scene
        if (fadeImage == null)
        {
            FindFadePanel();
        }

        // Fade in (from black)
        if (fadeImage != null)
        {
            yield return StartCoroutine(FadeIn());
        }
    }

    // Fade out (screen becomes black)
    private IEnumerator FadeOut()
    {
        if (!ValidateFadeImage()) yield break;

        Debug.Log("[SceneFader] Fading out...");
        float elapsedTime = 0f;
        Color c = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            if (fadeImage == null)
            {
                Debug.LogError("[SceneFader] FadeImage became null during FadeOut!");
                yield break;
            }

            elapsedTime += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        // Ensure fully opaque
        c.a = 1f;
        fadeImage.color = c;
        Debug.Log("[SceneFader] Fade out complete");
    }

    // Fade in (black screen becomes clear)
    private IEnumerator FadeIn()
    {
        if (!ValidateFadeImage()) yield break;

        Debug.Log("[SceneFader] Fading in...");
        float elapsedTime = 0f;
        Color c = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            if (fadeImage == null)
            {
                Debug.LogError("[SceneFader] FadeImage became null during FadeIn!");
                yield break;
            }

            elapsedTime += Time.deltaTime;
            c.a = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            fadeImage.color = c;
            yield return null;
        }

        // Ensure fully transparent
        c.a = 0f;
        fadeImage.color = c;
        Debug.Log("[SceneFader] Fade in complete");
    }

    // Just fade in (useful for scene start)
    public void FadeInOnly()
    {
        if (!ValidateFadeImage()) return;
        StartCoroutine(FadeIn());
    }

    // Just fade out (useful for scene end)
    public void FadeOutOnly()
    {
        if (!ValidateFadeImage()) return;
        StartCoroutine(FadeOut());
    }

    /// <summary>
    /// Validate that fadeImage is assigned and not null
    private bool ValidateFadeImage()
    {
        if (fadeImage == null)
        {
            // Try to find it one more time
            if (autoFindFadePanel)
            {
                FindFadePanel();
            }

            if (fadeImage == null)
            {
                Debug.LogError("[SceneFader] ERROR: Fade Image is NULL! Cannot perform fade. Please create a UI Image named 'FadePanel' in Canvas!");
                return false;
            }
        }
        return true;
    }
}
