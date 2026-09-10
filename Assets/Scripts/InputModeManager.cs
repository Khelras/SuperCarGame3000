using System;

public enum SteeringInputMode
{
    AnalogStick,
    RelativeTouch,
    Rotational
}

/// <summary>
/// Holds the player's currently selected steering input mode for the lifetime of the
/// application session. Not saved between sessions — always resets to AnalogStick on launch.
/// </summary>
public static class InputModeManager
{
    public static SteeringInputMode CurrentMode { get; private set; } = SteeringInputMode.AnalogStick;

    public static event Action<SteeringInputMode> OnModeChanged;

    public static void SetMode(SteeringInputMode mode)
    {
        if (mode == CurrentMode) return;

        CurrentMode = mode;
        OnModeChanged?.Invoke(CurrentMode);
    }

    public static void CycleMode()
    {
        int modeCount = Enum.GetValues(typeof(SteeringInputMode)).Length;
        int nextIndex = ((int)CurrentMode + 1) % modeCount;
        SetMode((SteeringInputMode)nextIndex);
    }
}