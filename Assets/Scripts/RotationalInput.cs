using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

/// <summary>
/// Rotational input for steering: reads the device's gyroscope attitude at runtime
/// and outputs a steering value based on roll relative to a calibrated zero orientation.
/// Call ResetRotation() to re-calibrate the zero point to the device's current orientation.
/// </summary>
public class RotationalInput : OnScreenControl
{
    [InputControl(layout = "Vector2")]
    [SerializeField] private string m_ControlPathInternal;
    protected override string controlPathInternal
    {
        get => m_ControlPathInternal;
        set => m_ControlPathInternal = value;
    }

    [Header("Sensitivity")]
    [SerializeField] private float maxTiltAngle = 30f; // Degrees of roll for full -1 to 1 steering output
    [SerializeField] private bool invert = false;
    public bool Invert
    {
        get => invert;
        set => invert = value;
    }

    private AttitudeSensor sensor;
    private Quaternion zeroAttitude = Quaternion.identity;
    private bool gyroSupported;

    protected override void OnEnable()
    {
        base.OnEnable(); // Required: registers this control with the Input System

        sensor = AttitudeSensor.current;
        gyroSupported = sensor != null;

        if (gyroSupported)
        {
            InputSystem.EnableDevice(sensor);
            ResetRotation();
        }
        else
        {
            Debug.LogWarning("RotationalInput: Gyroscope not supported on this device.");
        }
    }

    protected override void OnDisable()
    {
        if (gyroSupported && sensor != null)
            InputSystem.DisableDevice(sensor);

        base.OnDisable(); // Required: unregisters this control from the Input System
    }

    private void Update()
    {
        if (!gyroSupported) return;

        float roll = GetRelativeRoll();
        float steerValue = Mathf.Clamp(roll / maxTiltAngle, -1f, 1f);

        if (invert)
            steerValue = -steerValue;

        SendValueToControl(new Vector2(steerValue, 0f));
    }

    /// <summary>
    /// Resets the reference "zero" orientation to the device's current attitude.
    /// Hook this up to a Pause Menu button to let the player recalibrate their
    /// resting/neutral steering position at any time.
    /// </summary>
    public void ResetRotation()
    {
        if (!gyroSupported) return;
        zeroAttitude = ConvertGyroAttitude(sensor.attitude.ReadValue());
    }

    private float GetRelativeRoll()
    {
        Quaternion current = ConvertGyroAttitude(sensor.attitude.ReadValue());
        Quaternion relative = Quaternion.Inverse(zeroAttitude) * current;

        float roll = relative.eulerAngles.z;
        if (roll > 180f) roll -= 360f; // Normalize to -180 to 180

        return roll;
    }

    // Gyroscope space is right-handed; Unity uses a left-handed coordinate system.
    // This conversion is Unity's documented fix for that mismatch.
    private Quaternion ConvertGyroAttitude(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}