using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TextHighlightOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI text;
    public Color normalColor = Color.white;
    public Color highlightColor = Color.yellow;

    void Start()
    {
        text.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = normalColor;
    }
}
