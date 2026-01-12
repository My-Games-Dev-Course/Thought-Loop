using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

// Handles fade in/out transitions between scenes
public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Auto-Find Settings")]
    [SerializeField] private bool autoFindFadePanel = true;
    [SerializeField] private string fadePanelName = "FadePanel";

    private Coroutine currentFadeCoroutine;

    private void Awake()
    {
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

        if (fadeImage == null && autoFindFadePanel)
        {
            FindFadePanel();
        }

        if (fadeImage == null)
        {
            Debug.LogError("[SceneFader] Fade Image is NOT assigned and could not be found!");
            return;
        }

        ResetFadeImage();
    }

    private void ResetFadeImage()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            Debug.Log("[SceneFader] Reset fade image alpha to 0");
        }
    }

    private void FindFadePanel()
    {
        Debug.Log($"[SceneFader] Searching for FadePanel named '{fadePanelName}'...");

        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Canvas canvas in canvases)
        {
            Image[] images = canvas.GetComponentsInChildren<Image>(true);
            foreach (Image img in images)
            {
                if (img.name == fadePanelName)
                {
                    fadeImage = img;
                    Debug.Log($"[SceneFader] Found FadePanel in Canvas '{canvas.name}'");
                    return;
                }
            }
        }

        Debug.LogWarning($"[SceneFader] Could not find FadePanel named '{fadePanelName}'");
    }

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
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = null;
            Debug.Log("[SceneFader] Stopped previous fade coroutine on scene load");
        }

        ResetFadeImage();

        if (autoFindFadePanel && (fadeImage == null || !fadeImage))
        {
            Debug.Log($"[SceneFader] Scene '{scene.name}' loaded, re-searching for FadePanel...");
            FindFadePanel();
            ResetFadeImage();
        }
    }

    public void FadeToScene(string sceneName)
    {
        if (!ValidateFadeImage()) return;

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeAndLoadScene(sceneName));
    }

    public void FadeToScene(int sceneIndex)
    {
        if (!ValidateFadeImage()) return;

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeAndLoadScene(sceneIndex));
    }

    public void FadeToScene()
    {
        if (!ValidateFadeImage()) return;

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        Debug.Log($"[SceneFader] Starting fade to scene: {sceneName}");

        yield return StartCoroutine(FadeOut());

        SceneManager.LoadScene(sceneName);

        yield return null;

        if (fadeImage == null)
        {
            FindFadePanel();
        }

        if (fadeImage != null)
        {
            yield return StartCoroutine(FadeIn());
        }

        currentFadeCoroutine = null;
    }

    private IEnumerator FadeAndLoadScene(int sceneIndex)
    {
        Debug.Log($"[SceneFader] Starting fade to scene index: {sceneIndex}");

        yield return StartCoroutine(FadeOut());

        SceneManager.LoadScene(sceneIndex);

        yield return null;

        if (fadeImage == null)
        {
            FindFadePanel();
        }

        if (fadeImage != null)
        {
            yield return StartCoroutine(FadeIn());
        }

        currentFadeCoroutine = null;
    }

    private IEnumerator FadeAndLoadScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("[SceneFader] No next scene to load in build settings.");
            currentFadeCoroutine = null;
            yield break;
        }

        Debug.Log($"[SceneFader] Starting fade to next scene index: {nextSceneIndex}");

        yield return StartCoroutine(FadeOut());

        SceneManager.LoadScene(nextSceneIndex);

        yield return null;

        if (fadeImage == null)
        {
            FindFadePanel();
        }

        if (fadeImage != null)
        {
            yield return StartCoroutine(FadeIn());
        }

        currentFadeCoroutine = null;
    }

    // Fade to black - works even when game is paused
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

            elapsedTime += Time.unscaledDeltaTime;
            c.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;
        Debug.Log("[SceneFader] Fade out complete");
    }

    // Fade from black - works even when game is paused
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

            elapsedTime += Time.unscaledDeltaTime;
            c.a = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            fadeImage.color = c;
            yield return null;
        }

        c.a = 0f;
        fadeImage.color = c;
        Debug.Log("[SceneFader] Fade in complete");
    }

    public void FadeInOnly()
    {
        if (!ValidateFadeImage()) return;
        StartCoroutine(FadeIn());
    }

    public void FadeOutOnly()
    {
        if (!ValidateFadeImage()) return;
        StartCoroutine(FadeOut());
    }

    private bool ValidateFadeImage()
    {
        if (fadeImage == null)
        {
            if (autoFindFadePanel)
            {
                FindFadePanel();
            }

            if (fadeImage == null)
            {
                Debug.LogError("[SceneFader] ERROR: Fade Image is NULL!");
                return false;
            }
        }
        return true;
    }
}