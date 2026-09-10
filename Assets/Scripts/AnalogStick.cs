using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

/// <summary>
///     On-screen analog stick for touch input. Renders as two circles in screen space:
///     an outer circle defining the valid touch bounds, and an inner circle (handle)
///     that follows the touch position relative to the outer circle's center.
///     Touches starting outside the outer circle's radius are ignored entirely.
/// </summary>
public class AnalogStick : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [InputControl(layout = "Vector2")]
    [SerializeField] private string m_ControlPathInternal;
    protected override string controlPathInternal
    {
        get => m_ControlPathInternal;
        set => m_ControlPathInternal = value;
    }

    [Header("References")]
    [SerializeField] private RectTransform outerCircle;
    [SerializeField] private RectTransform innerCircle;

    private float outerRadius;
    private bool isDragging;

    private void Awake()
    {
        outerRadius = outerCircle.rect.width * 0.5f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 localPoint = ScreenToLocalPointInOuterCircle(eventData);

        // Ignore this touch entirely if it started outside the circle's bounds
        if (localPoint.magnitude > outerRadius)
        {
            isDragging = false;
            return;
        }

        isDragging = true;
        UpdateHandle(localPoint);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector2 localPoint = ScreenToLocalPointInOuterCircle(eventData);
        UpdateHandle(localPoint);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        innerCircle.anchoredPosition = Vector2.zero;
        SendValueToControl(Vector2.zero);
    }

    private Vector2 ScreenToLocalPointInOuterCircle(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            outerCircle, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        return localPoint;
    }

    private void UpdateHandle(Vector2 localPoint)
    {
        // Visual handle stays clamped to a circular shape for a natural stick feel
        Vector2 clampedVisualPoint = Vector2.ClampMagnitude(localPoint, outerRadius);
        innerCircle.anchoredPosition = clampedVisualPoint;

        // Output value: clamp X and Y independently so vertical tilt never
        // reduces the available horizontal range (ignores up/down for steering)
        float xValue = Mathf.Clamp(localPoint.x, -outerRadius, outerRadius) / outerRadius;
        float yValue = Mathf.Clamp(localPoint.y, -outerRadius, outerRadius) / outerRadius;

        SendValueToControl(new Vector2(xValue, yValue));
    }
}