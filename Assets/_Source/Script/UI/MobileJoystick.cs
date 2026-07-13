using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public static Vector2 Output { get; private set; }

    public RectTransform joystickBackground;
    public RectTransform joystickKnob;
    public float movementRange = 100f;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (joystickBackground == null || joystickKnob == null) return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground, 
            eventData.position, 
            eventData.pressEventCamera, 
            out localPoint))
        {
            Vector2 offset = localPoint;
            if (offset.magnitude > movementRange)
            {
                offset = offset.normalized * movementRange;
            }

            joystickKnob.anchoredPosition = offset;
            Output = offset / movementRange;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (joystickKnob != null)
            joystickKnob.anchoredPosition = Vector2.zero;
        
        Output = Vector2.zero;
    }
}
