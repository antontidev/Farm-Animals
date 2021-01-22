using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("UI/ExtendedButton")]
public class ExtendedButton : Button
{
    public UnityEvent OnClickUp;
    public UnityEvent OnClickDown;

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        OnClickUp?.Invoke();
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        OnClickDown?.Invoke();
    }
}