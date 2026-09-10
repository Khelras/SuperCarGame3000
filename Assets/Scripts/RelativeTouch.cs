using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

/// <summary>
/// Relative touch input for steering. Tracks the first touch that starts within a defined
/// bounds region, then reads how far the touch has moved from that initial point as the
/// input value. Draws a line between the start and current touch position while active.
/// </summary>
public class RelativeTouch : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [InputControl(layout = "Vector2")]
    [SerializeField] private string m_ControlPathInternal;
    protected override string controlPathInternal
    {
        get => m_ControlPathInternal;
        set => m_ControlPathInternal = value;
    }

    [Header("Bounds")]
    [SerializeField] private RectTransform touchBounds; // Valid area for the INITIAL touch only

    [Header("Sensitivity")]
    [SerializeField] private float maxDragDistance = 400; // Pixels of drag for full -1 to 1 output

    [Header("Visual Line")]
    [SerializeField] private RectTransform overlayContainer; // Full-stretch container the line lives in
    [SerializeField] private RectTransform lineImage;

    private Vector2 startScreenPos;
    private bool isDragging;
    private int activePointerId = -1;

    private void Awake()
    {
        // Set the Touch Bounds to be the Bottom-Left Quarter of the Screen
        touchBounds.anchorMin = new Vector2(0f, 0f);
        touchBounds.anchorMax = new Vector2(0.5f, 0.5f);
        touchBounds.offsetMin = Vector2.zero;
        touchBounds.offsetMax = Vector2.zero;

        if (lineImage != null)
            lineImage.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isDragging) return; // Already tracking a touch, ignore additional presses

        if (!RectTransformUtility.RectangleContainsScreenPoint(touchBounds, eventData.position, eventData.pressEventCamera))
            return; // Initial touch was outside bounds — ignore entirely

        isDragging = true;
        activePointerId = eventData.pointerId;
        startScreenPos = eventData.position;

        if (lineImage != null)
            lineImage.gameObject.SetActive(true);

        UpdateLine(startScreenPos, startScreenPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || eventData.pointerId != activePointerId) return;

        Vector2 delta = eventData.position - startScreenPos;
        Vector2 normalizedValue = new Vector2(
            Mathf.Clamp(delta.x / maxDragDistance, -1f, 1f),
            Mathf.Clamp(delta.y / maxDragDistance, -1f, 1f));

        SendValueToControl(normalizedValue);
        UpdateLine(startScreenPos, eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging || eventData.pointerId != activePointerId) return;

        isDragging = false;
        activePointerId = -1;
        SendValueToControl(Vector2.zero);

        if (lineImage != null)
            lineImage.gameObject.SetActive(false);
    }

    private void UpdateLine(Vector2 screenStart, Vector2 screenCurrent)
    {
        if (lineImage == null || overlayContainer == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(overlayContainer, screenStart, null, out Vector2 localStart);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(overlayContainer, screenCurrent, null, out Vector2 localCurrent);

        Vector2 direction = localCurrent - localStart;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        lineImage.anchoredPosition = localStart;
        lineImage.sizeDelta = new Vector2(distance, lineImage.sizeDelta.y);
        lineImage.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}