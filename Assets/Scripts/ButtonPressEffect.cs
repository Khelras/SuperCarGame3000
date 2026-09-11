using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private Image targetImage;

    [Header("Press Effect")]
    [SerializeField] private float pressDecrement = 0.05f;
    [SerializeField] private float pressDuration = 0.08f;
    [SerializeField] private Color pressedBackgroundColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private AnimationCurve pressCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pressSound;

    [Header("Release Action")]
    [SerializeField] private UnityEvent onReleaseActivate;

    // Pressing Variables
    private RectTransform parentRect;
    private Coroutine pressCoroutine;
    private Color baseColor;
    private bool isPressed;
    private float fullScale;
    private float pressScale;

    private void Awake()
    {
        // Parent Rectangle Transform Properties
        parentRect = GetComponent<RectTransform>();

        // Base Color
        if (targetImage != null) baseColor = targetImage.color;

        // Scale for Press Effect
        fullScale = parentRect.localScale.x;
        pressScale = parentRect.localScale.x - pressDecrement;

        if (audioSource != null) audioSource.ignoreListenerPause = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        // Reset back to the default button state
        if (pressCoroutine != null) StopCoroutine(pressCoroutine);

        // Force everything back to a clean default state
        isPressed = false;
        if (targetImage != null) targetImage.color = baseColor;
        parentRect.localScale = Vector3.one * fullScale;
    }

    public void SetBaseColor(Color newColor)
    {
        baseColor = newColor;

        // Apply immediately if the button isn't mid-press animation
        if (!isPressed && targetImage != null)
            targetImage.color = baseColor;
    }

    // ================================================== Press / Release ================================================== //
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        RestartPress(parentRect.localScale.x, pressScale, baseColor, pressedBackgroundColor);

        // Press SFX
        if (audioSource != null && pressSound != null) audioSource.PlayOneShot(pressSound);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isPressed) return;
        isPressed = false;

        bool releasedInsideButton = eventData.pointerCurrentRaycast.gameObject != null &&
            eventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(transform);

        // Fire the release action FIRST, so any color changes it triggers
        // are already applied before we animate back to the resting state
        if (releasedInsideButton)
        {
            onReleaseActivate?.Invoke();
        }

        // If the release action disabled this button (e.g. UI navigation, closing a menu),
        // don't try to animate a component/GameObject that's no longer active.
        if (!isActiveAndEnabled) return;
        RestartPress(parentRect.localScale.x, fullScale, pressedBackgroundColor, baseColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Cancel the press visually if the pointer drags off without releasing here
        if (isPressed)
        {
            isPressed = false;
            RestartPress(parentRect.localScale.x, fullScale, pressedBackgroundColor, baseColor);
        }
    }

    private void RestartPress(float fromScale, float toScale, Color fromColor, Color toColor)
    {
        if (pressCoroutine != null) StopCoroutine(pressCoroutine);
        pressCoroutine = StartCoroutine(PressRoutine(fromScale, toScale, fromColor, toColor));
    }

    private IEnumerator PressRoutine(float fromScale, float toScale, Color fromColor, Color toColor)
    {
        float elapsed = 0f;
        while (elapsed < pressDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = pressCurve.Evaluate(Mathf.Clamp01(elapsed / pressDuration));

            float scale = Mathf.Lerp(fromScale, toScale, t);
            parentRect.localScale = new Vector3(scale, scale, 2f);

            if (targetImage != null) targetImage.color = Color.Lerp(fromColor, toColor, t);
            yield return null;
        }

        parentRect.localScale = new Vector3(toScale, toScale, 2f);
        if (targetImage != null) targetImage.color = toColor;
    }
    // ================================================== Press / Release ================================================== //
}
