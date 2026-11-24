
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class BackButtonHover :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [Header("矢印の文字")]
    public TextMeshProUGUI arrowText;

    [Header("文字色")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    private void Start()
    {
        if (arrowText != null)
            arrowText.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (arrowText != null)
            arrowText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (arrowText != null)
            arrowText.color = normalColor;
    }
}
