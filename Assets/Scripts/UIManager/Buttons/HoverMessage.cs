using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverMessage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI messageText;

    void Start()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false); // Start hidden
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(true); // Show on hover
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false); // Hide when not hovering
        }
    }
}