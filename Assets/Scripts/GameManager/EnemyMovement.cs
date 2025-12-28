using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 3f;

    [Header("Edge Detection Settings")]
    [SerializeField] private float edgeCheckDistance = 0.3f; // How far ahead to check for edge
    [SerializeField] private float groundCheckDistance = 0.5f; // How far down to check for ground
    [SerializeField] private LayerMask groundLayer = 1; // Layer for ground/platforms (NOT player/clone)

    [Header("Enemy Dimensions")]
    [SerializeField] private float enemyHalfWidth = 0.5f; // Half width of enemy (auto-calculated if sprite renderer exists)
    [SerializeField] private float enemyHalfHeight = 0.5f; // Half height of enemy (auto-calculated if sprite renderer exists)
    //[SerializeField] private bool autoCalculateDimensions = true; // Auto-calculate from sprite renderer or collider

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;

    [Header("Collision Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string cloneTag = "Clone";

    private int direction = 1; // 1 for right, -1 for left

    void Start()
    {
        // Auto-get components if not assigned
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Start facing right
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = false;
        }
    }

    void Update()
    {
        // Stop movement when game is paused
        if (Time.timeScale == 0f)
        {
            return;
        }

        CheckForEdge();
        Move();
    }

    private void Move()
    {
        // Move in current direction
        float moveAmount = direction * speed * Time.deltaTime;
        transform.Translate(new Vector3(moveAmount, 0, 0));
    }

    private void CheckForEdge()
    {
        // Check if there's ground ahead in the direction we're moving
        // Position to check from (slightly ahead of enemy in movement direction)
        Vector2 checkPosition = new Vector2(
            transform.position.x + (direction * (enemyHalfWidth + edgeCheckDistance)),
            transform.position.y - enemyHalfHeight // Check from bottom of enemy
        );

        // Cast ray downward to check for ground ahead
        // Filter out player and clone from results
        RaycastHit2D hit = Physics2D.Raycast(checkPosition, Vector2.down, groundCheckDistance, groundLayer);

        // Ignore if hit player or clone
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag(playerTag) || hit.collider.CompareTag(cloneTag))
            {
                // Ignore player/clone, continue checking
                return;
            }
        }

        // If no valid ground detected ahead, we're at/near the edge - turn around
        if (hit.collider == null)
        {
            ChangeDirection();
            return;
        }

        // Also check for walls/obstacles directly ahead (only on ground layer)
        Vector2 wallCheckOrigin = new Vector2(
            transform.position.x + (direction * enemyHalfWidth),
            transform.position.y
        );

        RaycastHit2D wallHit = Physics2D.Raycast(wallCheckOrigin, new Vector2(direction, 0), edgeCheckDistance, groundLayer);

        // Ignore if hit player or clone
        if (wallHit.collider != null)
        {
            if (wallHit.collider.CompareTag(playerTag) || wallHit.collider.CompareTag(cloneTag))
            {
                // Ignore player/clone, don't change direction
                return;
            }

            // Hit a valid wall/obstacle on the ground layer, turn around
            ChangeDirection();
        }
    }

    private void ChangeDirection()
    {
        // Flip direction
        direction *= -1;

        // Flip sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction < 0;
        }

        Debug.Log($"[EnemyMovement] Changed direction to {(direction > 0 ? "right" : "left")}");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collided with player or clone - restart scene
        if (collision.gameObject.CompareTag(playerTag) ||
            collision.gameObject.CompareTag(cloneTag))
        {
            if (DeathTracker.Instance != null)
            {
                DeathTracker.Instance.RecordDeath();
            }

            Debug.Log($"[EnemyMovement] Collided with {collision.gameObject.tag}! Restarting level...");
            RestartScene();
            return;
        }

        // Check if collided with button (has PressureButton component) - change direction
        if (collision.gameObject.GetComponent<PressureButton>() != null)
        {
            ChangeDirection();
            return;
        }

        // If hit a wall or obstacle from the side, turn around
        ContactPoint2D contact = collision.contacts[0];
        if (Mathf.Abs(contact.normal.x) > 0.5f) // Hit from side (not top/bottom)
        {
            // Only change direction if we're hitting from the front
            if ((direction > 0 && contact.normal.x < 0) || (direction < 0 && contact.normal.x > 0))
            {
                ChangeDirection();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check triggers for player/clone collision
        if (other.CompareTag(playerTag) || other.CompareTag(cloneTag))
        {
            Debug.Log($"[EnemyMovement] Triggered with {other.tag}! Restarting level...");
            RestartScene();
            return;
        }

        // Check if triggered with button (has PressureButton component) - change direction
        if (other.GetComponent<PressureButton>() != null)
        {
            ChangeDirection();
        }
    }

    private void RestartScene()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}