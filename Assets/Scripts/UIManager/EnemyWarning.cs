using TMPro;
using UnityEngine;
using System.Collections;

// Displays a warning message when player/clone gets near enemy
// Shows every time (with cooldown to prevent spam)
public class EnemyWarning : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI warningText;

    [Header("Warning Settings")]
    [SerializeField] private string warningMessage = "Enemies are dangerous! Avoid them!";
    [SerializeField] private float displayDuration = 2.5f; // How long to show warning
    [SerializeField] private Color warningColor = Color.red;

    [Header("Detection Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string cloneTag = "Clone";
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float detectionDistance = 1.5f; // How close triggers warning

    private bool isShowingWarning = false;
    private GameObject player;

    void Start()
    {
        // Find the player
        player = GameObject.FindGameObjectWithTag(playerTag);

        if (player == null)
        {
            Debug.LogError("[EnemyWarning] Player not found! Make sure Player has 'Player' tag.");
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
            Debug.LogError("[EnemyWarning] Warning text not assigned!");
        }

        Debug.Log("[EnemyWarning] Initialized - watching for enemy proximity");
    }

    void Update()
    {
        // Only check if not already showing warning
        if (!isShowingWarning && player != null)
        {
            CheckForEnemyProximity();
        }
    }

    // Check if player or any clone is close to any enemy
    private void CheckForEnemyProximity()
    {
        // Get all enemies in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        if (enemies.Length == 0)
        {
            return; // No enemies, nothing to check
        }

        foreach (GameObject enemy in enemies)
        {
            // Check if player is close to this enemy
            if (player != null)
            {
                float playerDistance = Vector2.Distance(player.transform.position, enemy.transform.position);
                if (playerDistance < detectionDistance)
                {
                    Debug.Log($"[EnemyWarning] Player near enemy! Distance: {playerDistance}");
                    ShowWarning();
                    return;
                }
            }

            // Check if any clone is close to this enemy
            GameObject[] clones = GameObject.FindGameObjectsWithTag(cloneTag);
            foreach (GameObject clone in clones)
            {
                float cloneDistance = Vector2.Distance(clone.transform.position, enemy.transform.position);
                if (cloneDistance < detectionDistance)
                {
                    Debug.Log($"[EnemyWarning] Clone near enemy! Distance: {cloneDistance}");
                    ShowWarning();
                    return;
                }
            }
        }
    }

    // Display the warning message
    private void ShowWarning()
    {
        if (isShowingWarning) return;

        Debug.Log("[EnemyWarning] Enemy proximity detected! Showing warning");
        StartCoroutine(DisplayWarning());
    }

    // Show warning for a few seconds then hide
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

    // Public method to trigger warning manually if needed
    public void TriggerWarning()
    {
        ShowWarning();
    }

    // Change the warning message at runtime
    public void SetWarningMessage(string message)
    {
        warningMessage = message;
    }
}