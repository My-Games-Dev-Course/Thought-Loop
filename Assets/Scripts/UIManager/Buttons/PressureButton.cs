using UnityEngine;

public class PressureButton : MonoBehaviour
{

    [SerializeField] private bool isPressed = false;
    [SerializeField] private Animator animator;

    private int pressCount = 0; // To handle multiple objects pressing the button

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        isPressed = false;
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            pressCount++;
            Debug.Log("[PressureButton] pressCount: " + pressCount);
            isPressed = true;
            animator.SetBool("isPressed", true);
            Debug.Log("[PressureButton] Button pressed!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            pressCount--;
            Debug.Log("[PressureButton] pressCount: " + pressCount);
            if (pressCount <= 0)
            {
                isPressed = false;
                animator.SetBool("isPressed", false);
                Debug.Log("[PressureButton] Button released!");
            }
        }
    }

    public bool IsPressed()
    {
        return isPressed;
    }
}
