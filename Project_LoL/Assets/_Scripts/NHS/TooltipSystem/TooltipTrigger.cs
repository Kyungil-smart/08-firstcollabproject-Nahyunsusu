using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private  TooltipComponent _content;

    private void Awake()
    {
        _content = GetComponent<TooltipComponent>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_content != null && TooltipSystem.instance != null)
        {
            TooltipSystem.instance.Show(_content.GetTooltipText());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipSystem.instance != null)
        {
            TooltipSystem.instance.Hide();
        }
    }
}