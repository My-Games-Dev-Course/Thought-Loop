using UnityEngine;

public class MovingWall : MonoBehaviour
{

    [SerializeField] private PressureButton button;  // Assign in Inspector
    [SerializeField] private float moveDistance = 3f;  // How far up it moves
    [SerializeField] private float moveSpeed = 5f;  // How fast it moves

    private Vector2 closedPosition;
    private Vector2 openPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector2(0, moveDistance);
    }

    // Update is called once per frame
    void Update()
    {
        if (button.IsPressed())
        {
            // Move up (open)
            transform.position = Vector2.MoveTowards(transform.position, openPosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            // Move down (closed)
            transform.position = Vector2.MoveTowards(transform.position, closedPosition, moveSpeed * Time.deltaTime);
        }
    }
}

