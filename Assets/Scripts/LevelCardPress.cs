using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
///     Touch-friendly press feedback for level select cards:
///     shrinks slightly on press and returns to default on release,
///     only firing its action if the release point is still within the card's bounds.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class LevelCardPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] private Image backgroundImage;

    [Header("Press Settings")]
    [SerializeField] private float pressScale = 0.95f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pressedColor = Color.darkOrange;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pressSound;

    [Header("Animation")]
    [SerializeField] private float transitionDuration = 0.067f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public event System.Action OnCardActivated;

    private RectTransform rect;
    private Coroutine transitionCoroutine;
    private bool isPressed;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        rect.localScale = Vector3.one;

        if (backgroundImage != null)
            backgroundImage.color = normalColor;

        if (audioSource != null)
            audioSource.ignoreListenerPause = true;
    }

    private void OnDisable()
    {
        // Reset cleanly if the canvas/card is disabled mid-press
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        isPressed = false;
        rect.localScale = Vector3.one;

        if (backgroundImage != null)
            backgroundImage.color = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        TransitionTo(pressScale, pressedColor);

        if (audioSource != null && pressSound != null)
            audioSource.PlayOneShot(pressSound);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isPressed) return;
        isPressed = false;

        TransitionTo(1f, normalColor);

        bool releasedInsideBounds = RectTransformUtility.RectangleContainsScreenPoint(
            rect, eventData.position, eventData.pressEventCamera);

        if (releasedInsideBounds)
        {
            OnCardActivated?.Invoke();
        }
    }

    private void TransitionTo(float targetScale, Color targetColor)
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(TransitionRoutine(targetScale, targetColor));
    }

    private IEnumerator TransitionRoutine(float targetScale, Color targetColor)
    {
        float startScale = rect.localScale.x;
        Color startColor = backgroundImage != null ? backgroundImage.color : Color.white;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = transitionCurve.Evaluate(Mathf.Clamp01(elapsed / transitionDuration));

            float scale = Mathf.Lerp(startScale, targetScale, t);
            rect.localScale = new Vector3(scale, scale, 1f);

            if (backgroundImage != null)
                backgroundImage.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        rect.localScale = new Vector3(targetScale, targetScale, 1f);
        if (backgroundImage != null)
            backgroundImage.color = targetColor;
    }
}