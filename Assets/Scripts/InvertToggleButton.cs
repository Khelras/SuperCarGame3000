using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Toggle button for RotationalInput's invert setting. Changes background color
/// between two states to visually indicate whether inversion is currently active.
/// </summary>
public class InvertToggleButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RotationalInput rotationalInput;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private ButtonPressEffect buttonPressEffect;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color invertedColor = Color.red;

    private void OnEnable()
    {
        UpdateVisual();
    }

    /// <summary>
    /// Hook this up to ButtonPressEffect's onReleaseActivate UnityEvent.
    /// </summary>
    public void ToggleInvert()
    {
        if (rotationalInput == null) return;

        rotationalInput.Invert = !rotationalInput.Invert;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (backgroundImage == null || rotationalInput == null) return;

        Color targetColor = rotationalInput.Invert ? invertedColor : normalColor;

        if (buttonPressEffect != null)
            buttonPressEffect.SetBaseColor(targetColor);
        else
            backgroundImage.color = targetColor;
    }
}