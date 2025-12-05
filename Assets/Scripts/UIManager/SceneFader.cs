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


    private void FindFadePanel()
    {
        Debug.Log($"[SceneFader] Searching for FadePanel named '{fadePanelName}'...");

        // Search all canvases, including inactive children
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);  // updated to use FindObjectsByType
        foreach (Canvas canvas in canvases)
        {
            Image[] images = canvas.GetComponentsInChildren<Image>(true); // include inactive
            foreach (Image img in images)
            {
                if (img.name == fadePanelName)
                {
                    fadeImage = img;
                    Debug.Log($"[SceneFader] ✅ Found FadePanel in Canvas '{canvas.name}' at path '{(img.transform)}'");
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
        // If the current fadeImage got destroyed on scene unload, re-find it.
        if (autoFindFadePanel && (fadeImage == null || !fadeImage))
        {
            Debug.Log($"[SceneFader] Scene '{scene.name}' loaded, re-searching for FadePanel...");
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

    // Load current scene with fade according to next scene set in SceneManager
    public void FadeToScene()
    {
        if (!ValidateFadeImage()) return;
        StartCoroutine(FadeAndLoadScene());
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

    // For loading also automatically the next scene in build settings

    private IEnumerator FadeAndLoadScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("[SceneFader] No next scene to load in build settings.");
            yield break;
        }
        Debug.Log($"[SceneFader] Starting fade to next scene index: {nextSceneIndex}");
        // Fade out (to black)
        yield return StartCoroutine(FadeOut());
        // Load the next scene
        SceneManager.LoadScene(nextSceneIndex);
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

    // Validate that fadeImage is assigned and not null
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
