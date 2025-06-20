using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private Image joystickBackground;
    [SerializeField] private Image joystickKnob;
    [SerializeField] private float joystickRadius = 100f;
    private Vector2 inputDirection;

    private void Start()
    {
        inputDirection = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickKnob.rectTransform.anchoredPosition = Vector2.zero;
        inputDirection = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground.rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 position
        );

        position = Vector2.ClampMagnitude(position, joystickRadius);
        joystickKnob.rectTransform.anchoredPosition = position;
        inputDirection = position / joystickRadius;
    }

    public float Horizontal()
    {
        return inputDirection.x;
    }

    public float Vertical()
    {
        return inputDirection.y;
    }

    public float Magnitude()
    {
        return inputDirection.magnitude;
    }

    public void ResetJoystick()
    {
        joystickKnob.rectTransform.anchoredPosition = Vector2.zero;
        inputDirection = Vector2.zero;
    }
}

