using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class LevelCardHoverPress : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private Image backgroundImage;

    [Header("Hover Settings")]
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.darkOrange;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip pressSound;

    [Header("Animation")]
    [SerializeField] private float transitionDuration = 0.12f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform rect;
    private Coroutine transitionCoroutine;
    private bool isHovering;
    private bool isPressed;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        rect.localScale = Vector3.one;

        if (backgroundImage != null) backgroundImage.color = normalColor;

        if (audioSource != null) audioSource.ignoreListenerPause = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        if (!isPressed) TransitionTo(hoverScale, hoverColor);

        // Hover SFX
        if (audioSource != null && hoverSound != null) audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        isPressed = false; // dragging off cancels the press state too

        TransitionTo(1f, normalColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        TransitionTo(1f, normalColor);

        // Press SFX
        if (audioSource != null && pressSound != null) audioSource.PlayOneShot(pressSound);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isPressed) return;
        isPressed = false;

        // Return to hover state if still hovering, otherwise stay default
        if (isHovering) TransitionTo(hoverScale, hoverColor);
        else TransitionTo(1f, normalColor);
    }

    private void TransitionTo(float targetScale, Color targetColor)
    {
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(TransitionRoutine(targetScale, targetColor));
    }

    private IEnumerator TransitionRoutine(float targetScale, Color targetColor)
    {
        float startScale = rect.localScale.x;
        Color startColor = backgroundImage != null ? backgroundImage.color : Color.white;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = transitionCurve.Evaluate(Mathf.Clamp01(elapsed / transitionDuration));

            float scale = Mathf.Lerp(startScale, targetScale, t);
            rect.localScale = new Vector3(scale, scale, 1f);

            if (backgroundImage != null) backgroundImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        rect.localScale = new Vector3(targetScale, targetScale, 1f);
        if (backgroundImage != null) backgroundImage.color = targetColor;
    }
}