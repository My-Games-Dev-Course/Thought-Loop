using TMPro;
using UnityEngine;
using System.Collections;

/// <summary>
/// Displays a warning message when player touches clone
/// Non-intrusive - doesn't pause game, just shows text briefly
/// </summary>
public class CloneCollisionWarning : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI warningText;

    [Header("Warning Settings")]
    [SerializeField] private string warningMessage = "Don't touch your clone!";
    [SerializeField] private float displayDuration = 2f; // How long to show warning
    [SerializeField] private Color warningColor = Color.red;

    [Header("Detection Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string cloneTag = "Clone";
    //[SerializeField] private float checkInterval = 0.1f; // How often to check for collisions

    private bool isShowingWarning = false;
    private GameObject player;

    void Start()
    {
        // Find the player
        player = GameObject.FindGameObjectWithTag(playerTag);

        if (player == null)
        {
            Debug.LogError("[CloneCollisionWarning] Player not found! Make sure Player has 'Player' tag.");
        }

        // Setup warning text
        if (warningText != null)
        {
            warningText.text = "";
            warningText.color = warningColor;
            warningText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("[CloneCollisionWarning] Warning text not assigned!");
        }

        Debug.Log("[CloneCollisionWarning] Initialized - watching for clone collisions");
    }

    void Update()
    {
        // Only check if not already showing warning
        if (!isShowingWarning && player != null)
        {
            CheckForCloneCollision();
        }
    }

    /// <summary>
    /// Check if player is touching any clone
    /// Uses Physics2D overlap to detect collision
    /// </summary>
    private void CheckForCloneCollision()
    {
        // Get all clones in the scene
        GameObject[] clones = GameObject.FindGameObjectsWithTag(cloneTag);

        foreach (GameObject clone in clones)
        {
            // Check if player and clone are close enough (touching)
            float distance = Vector2.Distance(player.transform.position, clone.transform.position);

            // If very close (less than 1 unit), they're probably colliding
            if (distance < 1.2f)
            {
                ShowWarning();
                break; // Only show warning once
            }
        }
    }

    /// <summary>
    /// Display the warning message
    /// </summary>
    private void ShowWarning()
    {
        if (isShowingWarning) return;

        Debug.Log("[CloneCollisionWarning] Clone collision detected! Showing warning");
        StartCoroutine(DisplayWarning());
    }

    /// <summary>
    /// Show warning for a few seconds then hide
    /// </summary>
    private IEnumerator DisplayWarning()
    {
        isShowingWarning = true;

        // Show the warning text
        if (warningText != null)
        {
            warningText.text = warningMessage;
            warningText.gameObject.SetActive(true);
        }

        // Wait for display duration
        yield return new WaitForSeconds(displayDuration);

        // Hide the warning
        if (warningText != null)
        {
            warningText.text = "";
            warningText.gameObject.SetActive(false);
        }

        // Allow showing again after a cooldown
        yield return new WaitForSeconds(0.5f);
        isShowingWarning = false;
    }

    /// <summary>
    /// Public method to trigger warning manually if needed
    /// </summary>
    public void TriggerWarning()
    {
        ShowWarning();
    }

    /// <summary>
    /// Change the warning message at runtime
    /// </summary>
    public void SetWarningMessage(string message)
    {
        warningMessage = message;
    }
}