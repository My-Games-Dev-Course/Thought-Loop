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