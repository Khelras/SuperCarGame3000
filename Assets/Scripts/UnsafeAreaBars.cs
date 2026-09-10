using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Draws black bars over any screen area outside the Safe Area
/// and keeping them in sync with Safe Area's anchors.
/// </summary>
public class UnsafeAreaBars : MonoBehaviour
{
    [SerializeField] private RectTransform barTop;
    [SerializeField] private RectTransform barBottom;
    [SerializeField] private RectTransform barLeft;
    [SerializeField] private RectTransform barRight;

    private Rect lastSafeArea = new Rect(0, 0, 0, 0);

    private void Awake()
    {
        ApplyBars();
    }

    private void Update()
    {
        if (Screen.safeArea != lastSafeArea)
        {
            ApplyBars();
        }
    }

    private void ApplyBars()
    {
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;

        float screenW = Screen.width;
        float screenH = Screen.height;

        // Top bar: fills space above the safe area
        SetBar(barTop, new Vector2(0f, safeArea.yMax / screenH), new Vector2(1f, 1f));

        // Bottom bar: fills space below the safe area
        SetBar(barBottom, new Vector2(0f, 0f), new Vector2(1f, safeArea.yMin / screenH));

        // Left bar: fills space left of the safe area
        SetBar(barLeft, new Vector2(0f, 0f), new Vector2(safeArea.xMin / screenW, 1f));

        // Right bar: fills space right of the safe area
        SetBar(barRight, new Vector2(safeArea.xMax / screenW, 0f), new Vector2(1f, 1f));
    }

    private void SetBar(RectTransform bar, Vector2 anchorMin, Vector2 anchorMax)
    {
        if (bar == null) return;
        bar.anchorMin = anchorMin;
        bar.anchorMax = anchorMax;
        bar.offsetMin = Vector2.zero;
        bar.offsetMax = Vector2.zero;
    }
}