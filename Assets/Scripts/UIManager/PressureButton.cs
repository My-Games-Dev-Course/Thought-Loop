using UnityEngine;

public class PressureButton : MonoBehaviour
{

    [SerializeField] private bool isPressed = false;
    [SerializeField] private Animator animator;

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
            isPressed = true;
            animator.SetBool("isPressed", true);
            Debug.Log("[PressureButton] Button pressed!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Clone"))
        {
            isPressed = false;
            animator.SetBool("isPressed", false);
            Debug.Log("[PressureButton] Button released!");
        }
    }

    public bool IsPressed()
    {
        return isPressed;
    }
}
