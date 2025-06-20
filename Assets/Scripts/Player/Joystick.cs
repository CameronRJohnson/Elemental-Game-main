using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private Image joystickBackground;  // The background of the joystick
    [SerializeField] private Image joystickKnob;        // The knob of the joystick
    [SerializeField] private float joystickRadius = 100f; // Radius limit for the knob movement
    private Vector2 inputDirection;    // Direction of the joystick movement

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
        // Reset knob position and input direction when released
        joystickKnob.rectTransform.anchoredPosition = Vector2.zero;
        inputDirection = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Get the position of the pointer relative to the joystick background
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground.rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 position
        );

        // Normalize the position within the joystick radius
        position = Vector2.ClampMagnitude(position, joystickRadius);

        // Move the joystick knob based on pointer position
        joystickKnob.rectTransform.anchoredPosition = position;

        // Calculate input direction and magnitude (distance from center)
        inputDirection = position / joystickRadius;
    }

    // Function to get the horizontal input
    public float Horizontal()
    {
        return inputDirection.x;
    }

    // Function to get the vertical input
    public float Vertical()
    {
        return inputDirection.y;
    }

    public float Magnitude()
    {
        return inputDirection.magnitude; // Returns distance of knob from center, between 0 and 1
    }

    public void ResetJoystick()
    {
        joystickKnob.rectTransform.anchoredPosition = Vector2.zero;
        inputDirection = Vector2.zero;
    }
}
