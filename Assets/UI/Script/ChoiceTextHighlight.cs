using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ChoiceTextHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI text;
    private Color originalColor;
    public Color hoverColor;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
            originalColor = text.color;
    }

    void OnEnable()
    {
        if (text != null)
        {
            text.color = originalColor;
        }        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (text != null)
            text.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (text != null)
            text.color = originalColor;
    }
}
