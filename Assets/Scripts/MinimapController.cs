using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RaceManager raceManager;
    [SerializeField] private RectTransform playerIcon;
    [SerializeField] private RectTransform minimapRect;

    [Header("Padding")]
    [SerializeField] private float boundsPadding = 1.1f; // 10% margin so the icon never touches the map edge

    private Vector2 worldMin;
    private Vector2 worldMax;

    private void Start()
    {
        worldMin = raceManager.GetTrackWorldMin();
        worldMax = raceManager.GetTrackWorldMax();

        // Expand bounds outward slightly using the padding factor
        Vector2 center = (worldMin + worldMax) * 0.5f;
        worldMin = center + (worldMin - center) * boundsPadding;
        worldMax = center + (worldMax - center) * boundsPadding;
    }

    private void Update()
    {
        UpdatePlayerIconPosition();
        UpdatePlayerIconRotation();
    }

    private void UpdatePlayerIconPosition()
    {
        Vector3 carPos = raceManager.m_PlayerCar.position;

        float normalizedX = Mathf.InverseLerp(worldMin.x, worldMax.x, carPos.x);
        float normalizedZ = Mathf.InverseLerp(worldMin.y, worldMax.y, carPos.z);

        float uiX = (normalizedX - 0.5f) * minimapRect.rect.width;
        float uiY = (normalizedZ - 0.5f) * minimapRect.rect.height;

        playerIcon.anchoredPosition = new Vector2(uiX, uiY);
    }

    private void UpdatePlayerIconRotation()
    {
        float carYaw = raceManager.m_PlayerCar.rotation.eulerAngles.y;
        playerIcon.rotation = Quaternion.Euler(0f, 0f, -carYaw);
    }
}