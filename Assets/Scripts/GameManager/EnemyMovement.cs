using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 3f;

    [Header("Edge Detection Settings")]
    [SerializeField] private float edgeCheckDistance = 0.3f;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer = 1;

    [Header("Enemy Dimensions")]
    [SerializeField] private float enemyHalfWidth = 0.5f;
    [SerializeField] private float enemyHalfHeight = 0.5f;

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
        Vector2 checkPosition = new Vector2(
            transform.position.x + (direction * (enemyHalfWidth + edgeCheckDistance)),
            transform.position.y - enemyHalfHeight
        );

        // Cast ray downward to check for ground ahead
        RaycastHit2D hit = Physics2D.Raycast(checkPosition, Vector2.down, groundCheckDistance, groundLayer);

        // Ignore if hit player or clone
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag(playerTag) || hit.collider.CompareTag(cloneTag))
            {
                return;
            }
        }

        // If no valid ground detected ahead, turn around
        if (hit.collider == null)
        {
            ChangeDirection();
            return;
        }

        // Check for walls/obstacles directly ahead
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
                return;
            }

            // Hit a valid wall, turn around
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
        // Check if collided with player or clone
        if (collision.gameObject.CompareTag(playerTag) ||
            collision.gameObject.CompareTag(cloneTag))
        {
            if (DeathTracker.Instance != null)
            {
                DeathTracker.Instance.RecordDeath();
            }

            Debug.Log($"[EnemyMovement] Collided with {collision.gameObject.tag}! Restarting level...");

            // Freeze everything immediately
            Time.timeScale = 0f;

            // Show death message
            if (DeathMessage.Instance != null)
            {
                DeathMessage.Instance.ShowDeathMessage("Killed by enemy!");
            }
            else
            {
                // Fallback if no death message system
                Time.timeScale = 1f;
                RestartScene();
            }
            return;
        }

        // Check if collided with button - change direction
        if (collision.gameObject.GetComponent<PressureButton>() != null)
        {
            ChangeDirection();
            return;
        }

        // If hit a wall from the side, turn around
        ContactPoint2D contact = collision.contacts[0];
        if (Mathf.Abs(contact.normal.x) > 0.5f)
        {
            // Only change direction if hitting from the front
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
            if (DeathTracker.Instance != null)
            {
                DeathTracker.Instance.RecordDeath();
            }

            Debug.Log($"[EnemyMovement] Triggered with {other.tag}! Restarting level...");

            // Freeze everything immediately
            Time.timeScale = 0f;

            // Show death message
            if (DeathMessage.Instance != null)
            {
                DeathMessage.Instance.ShowDeathMessage("Killed by enemy!");
            }
            else
            {
                // Fallback if no death message system
                Time.timeScale = 1f;
                RestartScene();
            }
            return;
        }

        // Check if triggered with button - change direction
        if (other.GetComponent<PressureButton>() != null)
        {
            ChangeDirection();
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}