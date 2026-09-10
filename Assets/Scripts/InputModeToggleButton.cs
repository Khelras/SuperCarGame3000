using TMPro;
using UnityEngine;

/// <summary>
/// Cycles through the available steering input modes and updates its own label text
/// to reflect the currently selected mode. Used identically in both the Main Menu
/// and the Pause Menu, kept in sync via InputModeManager's static state.
/// </summary>
public class InputModeToggleButton : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    private void OnEnable()
    {
        UpdateLabel(InputModeManager.CurrentMode);
        InputModeManager.OnModeChanged += UpdateLabel;
    }

    private void OnDisable()
    {
        InputModeManager.OnModeChanged -= UpdateLabel;
    }

    /// <summary>
    /// Hook this up to ButtonPressEffect's onReleaseActivate UnityEvent.
    /// </summary>
    public void CycleInputMode()
    {
        InputModeManager.CycleMode();
    }

    private void UpdateLabel(SteeringInputMode mode)
    {
        if (label == null) return;

        label.text = mode switch
        {
            SteeringInputMode.AnalogStick => "  Input: Analog",
            SteeringInputMode.RelativeTouch => "  Input: Relative",
            SteeringInputMode.Rotational => "  Input: Rotational",
            _ => mode.ToString()
        };
    }
}