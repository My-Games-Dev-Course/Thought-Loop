using UnityEngine;
using UnityEngine.UI;

/**
 * Hint Button - Shows a hint popup when clicked
 * Similar to ResetButton but shows hint instead of resetting
 */
public class HintButton : MonoBehaviour
{

    [SerializeField] private string hintMessage = "Your hint here!";

    public void ShowHint()
    {
        HintPopup.Instance.ShowHint(hintMessage);
    }
}

//using UnityEngine;
//using UnityEngine.UI;

//public class HintButton : MonoBehaviour
//{
//    [Header("Hint Content")]
//    [TextArea(3, 10)]
//    [SerializeField] private string hintMessage = "After 10 second a clone will appear";

//    [Header("References")]
//    [SerializeField] private Button button;

//    void Start()
//    {
//        // Auto-find button if not assigned
//        if (button == null)
//            button = GetComponent<Button>();

//        // Setup button click
//        if (button != null)
//        {
//            button.onClick.AddListener(OnButtonClicked);
//        }
//        else
//        {
//            Debug.LogError("[HintButton] No Button component found!");
//        }
//    }

//    private void OnButtonClicked()
//    {
//        Debug.Log("[HintButton] Hint button clicked!");

//        // Show popup with the hint message
//        if (HintPopup.Instance != null)
//        {
//            HintPopup.Instance.ShowHint(hintMessage);
//        }
//        else
//        {
//            Debug.LogError("[HintButton] HintPopup.Instance not found!");
//        }
//    }

//    void OnDestroy()
//    {
//        if (button != null)
//        {
//            button.onClick.RemoveListener(OnButtonClicked);
//        }
//    }
//}