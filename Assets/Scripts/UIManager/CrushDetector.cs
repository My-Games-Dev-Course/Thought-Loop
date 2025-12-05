using UnityEngine;
using UnityEngine.SceneManagement;

public class CrushDetector : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag to detect (usually 'Player')")]
    [SerializeField] private string targetTag = "Player";

    //[Tooltip("Should it also detect Clone?")]
    //[SerializeField] private bool detectClone = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if Player touched the crush detector
        if (other.CompareTag(targetTag))
        {
            Debug.Log("[CrushDetector] Player crushed! Restarting scene...");
            RestartScene();
        }

        //// Optionally detect clone too
        //if (detectClone && other.CompareTag("Clone"))
        //{
        //    Debug.Log("[CrushDetector] Clone crushed! Restarting scene...");
        //    RestartScene();
        //}
    }

    private void RestartScene()
    {
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}