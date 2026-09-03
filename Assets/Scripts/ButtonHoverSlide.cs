using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class ButtonHoverSlide : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private RectTransform hoverRect;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Image hoverImage;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;

    [Header("Hover Slide")]
    [SerializeField] private float slideDuration = 0.15f;
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color hoverTextColor = Color.black;

    // Sliding Variables
    private RectTransform parentRect;
    private Coroutine slideCoroutine;
    private float fullWidth;
    private bool isHovering;

    private void Awake()
    {
        // Parent Rectangle Transform Properties
        parentRect = GetComponent<RectTransform>();

        // Left-Anchor for Hover Slide Animation
        hoverRect.anchorMin = new Vector2(0f, 0f);
        hoverRect.anchorMax = new Vector2(0f, 1f);
        hoverRect.pivot = new Vector2(0f, 0.5f);
        hoverRect.anchoredPosition = Vector2.zero;

        SetHoverWidth(0f);

        if (buttonText != null) buttonText.color = normalTextColor;

        if (audioSource != null) audioSource.ignoreListenerPause = true;
    }

    private void Start()
    {
        fullWidth = parentRect.rect.width;
    }

    private void OnDisable()
    {
        // Stop any in-progress animation so it doesn't fight the reset
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);

        // Force everything back to a clean default state
        isHovering = false;
        if (buttonText != null)buttonText.color = normalTextColor;
        SetHoverWidth(0f);
        parentRect.localScale = Vector3.one; 
    }

    // ================================================== Hover ================================================== //
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Reset width to the full width
        isHovering = true;
        RestartSlide(hoverRect.sizeDelta.x, fullWidth, hoverTextColor);

        // Hover SFX
        if (audioSource != null && hoverSound != null) audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Reset width to 0
        isHovering = false;
        RestartSlide(hoverRect.sizeDelta.x, 0f, normalTextColor);
    }

    private void RestartSlide(float from, float to, Color textColor)
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideRoutine(from, to, textColor));
    }

    private IEnumerator SlideRoutine(float from, float to, Color textColor)
    {
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = slideCurve.Evaluate(Mathf.Clamp01(elapsed / slideDuration));
            SetHoverWidth(Mathf.Lerp(from, to, t));

            if (buttonText != null) buttonText.color = Color.Lerp(buttonText.color, textColor, t);
            yield return null;
        }

        SetHoverWidth(to);
        if (buttonText != null) buttonText.color = textColor;
    }

    private void SetHoverWidth(float width)
    {
        Vector2 size = hoverRect.sizeDelta;
        size.x = width;
        hoverRect.sizeDelta = size;
    }
    // ================================================== Hover ================================================== //
}