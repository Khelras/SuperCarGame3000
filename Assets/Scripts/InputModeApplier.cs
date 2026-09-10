using UnityEngine;

/// <summary>
/// Enables the on-screen control GameObject matching the player's selected steering
/// input mode when the Game scene starts, and reacts live if the mode is changed
/// mid-race via the Pause Menu.
/// </summary>
public class InputModeApplier : MonoBehaviour
{
    [SerializeField] private GameObject analogStickObject;
    [SerializeField] private GameObject relativeTouchObject;
    [SerializeField] private GameObject rotationalObject;

    private void Start()
    {
        ApplyMode(InputModeManager.CurrentMode);
    }

    private void OnEnable()
    {
        InputModeManager.OnModeChanged += ApplyMode;
    }

    private void OnDisable()
    {
        InputModeManager.OnModeChanged -= ApplyMode;
    }

    private void ApplyMode(SteeringInputMode mode)
    {
        analogStickObject.SetActive(mode == SteeringInputMode.AnalogStick);
        relativeTouchObject.SetActive(mode == SteeringInputMode.RelativeTouch);
        rotationalObject.SetActive(mode == SteeringInputMode.Rotational);
    }
}