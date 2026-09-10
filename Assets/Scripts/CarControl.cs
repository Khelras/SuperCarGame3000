using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CarControl : MonoBehaviour
{
    [Header("External Components")]
    public RaceManager m_RaceManager;

    [Header("Internal Components")]
    public Rigidbody m_Body;
    public WheelControl[] m_Wheels;

    public Material m_ReverseLightMaterial;
    public Light[] m_ReverseLights;

    public ParticleSystem m_Exhaust;

    public ManagedAudioSource m_EngineSound;
    public ManagedAudioSource m_ReverseSound;
    public AudioSource m_CrashSound;

    [Header("Car Properties")]
    public float m_MotorTorque = 500.0f;
    public float m_BrakeTorque = 200.0f;
    public float m_MaxSpeed = 50.0f;

    public float m_SteeringRange = 30.0f;
    public float m_SteeringRangeAtMaxSpeed = 10.0f;

    public float m_SteerAcceleration = 4.0f;
    public float m_SteerReturnAcceleration = 2.0f;
    private float m_CurrentTurn = 0.0f;
    
    [Header("ReverseLights")]
    public Vector2 m_ReverseLightEmissiveStrengths = new Vector2(1.0f, 8.0f);
    public Vector2 m_ReverseLightStrengths = new Vector2(0.0f, 5.0f);
    public float m_ReverseLightLerp = 8.0f;
    private float m_CurrentLightEmissive = 0.0f;
    private float m_CurrentLightStrength = 0.0f;

    [Header("Crash")]
    public Vector2 m_CrashPitchRange = Vector2.one;
    public Vector2 m_CrashVolumeRange = Vector2.one;

    [Header("Input Actions")]
    public InputActionAsset m_InputActions;

    private bool m_IsControllable = false;
    public bool Controllable
    {
        get { return m_IsControllable; }
        set { m_IsControllable = value; }
    }

    public bool m_MouseLocked = false;

    /// <summary>
    /// Gizmo callback so that we can render appropriate scene gizmos
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(m_Body.worldCenterOfMass, 0.1f);
    }

    /// <summary>
    /// Editor-only function that is called when something changes, the scene starts, and 
    /// some other convenient locations/times
    /// </summary>
    private void OnValidate()
    {
        // Automatically attach rigidbody
        if (!m_Body)
        {
            m_Body = GetComponent<Rigidbody>();
        }

        // Find the race manager
        if (!m_RaceManager)
        {
            m_RaceManager = FindAnyObjectByType<RaceManager>();
        }
    }

    /// <summary>
    /// Called when this gameObject is first made active
    /// </summary>
    private void Start()
    {
        m_InputActions.Enable();

        m_CurrentLightEmissive = m_ReverseLightEmissiveStrengths.x;
        m_ReverseLightMaterial.SetFloat("_EmissiveStrength", m_CurrentLightEmissive);

        m_CurrentLightStrength = m_ReverseLightStrengths.x;
        for (int i = 0; i < m_ReverseLights.Length; i++)
        {
            m_ReverseLights[i].intensity = m_CurrentLightStrength;
        }

        LockMouse();
    }

    /// <summary>
    /// Small helper function to handle cursor behaviour
    /// </summary>
    private void LockMouse()
    {
        Cursor.lockState = m_MouseLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !m_MouseLocked;
    }

    /// <summary>
    /// Our update loop from Unity, called as fast as possible 
    /// </summary>
    private void Update()
    {
        if (m_InputActions.FindAction("ToggleMouse").WasPressedThisFrame())
        {
            m_MouseLocked = !m_MouseLocked;
            LockMouse();
        }
        
        //if (m_InputActions.FindAction("Quit").WasPressedThisFrame())
        //{
        //    SceneManager.LoadScene("Menu");
        //}

        if (m_InputActions.FindAction("KillCar").WasPressedThisFrame())
        {
            m_RaceManager.BeginCarRespawn();
        }

        if (m_InputActions.FindAction("ReloadLevel").WasPressedThisFrame())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    /// <summary>
    /// Similar to Update, but called at a fixed time interval
    /// Use for physics-related calls
    /// </summary>
    void FixedUpdate()
    {
        // Exit out if we're currently not controllable (important at beginning/end of race)
        if (!m_IsControllable)
        {
            return;
        }

        // Get player input for acceleration and steering
        float vInput = m_InputActions.FindAction("Throttle").ReadValue<float>(); // Forward/backward input
        float hInput = m_InputActions.FindAction("Steer").ReadValue<float>(); // Steering input

        //// Slowly turn our steering
        //if (Mathf.Abs(hInput) > 0.1f)
        //{
        //    m_CurrentTurn += hInput * m_SteerAcceleration * Time.fixedDeltaTime;
        //}
        //else
        //{
        //    int sign = Mathf.RoundToInt(Mathf.Sign(m_CurrentTurn));
        //    m_CurrentTurn -= Mathf.Sign(m_CurrentTurn) * m_SteerReturnAcceleration * Time.fixedDeltaTime;

        //    if (sign != Mathf.RoundToInt(Mathf.Sign(m_CurrentTurn)))
        //    {
        //        m_CurrentTurn = 0.0f;
        //    }
        //}
        //m_CurrentTurn = Mathf.Clamp(m_CurrentTurn, -1.0f, 1.0f);

        // New Turning Logic for Mobile Inputs:
        // Smoothly move toward the target steering input (magnitude-aware)
        float targetTurn = hInput;
        float acceleration = Mathf.Abs(targetTurn) > Mathf.Abs(m_CurrentTurn)
            ? m_SteerAcceleration
            : m_SteerReturnAcceleration;

        m_CurrentTurn = Mathf.MoveTowards(m_CurrentTurn, targetTurn, acceleration * Time.fixedDeltaTime);
        m_CurrentTurn = Mathf.Clamp(m_CurrentTurn, -1.0f, 1.0f);

        // Calculate current speed along the car's forward axis
        float forwardSpeed = Vector3.Dot(transform.forward, m_Body.linearVelocity);
        float speedFactor = Mathf.InverseLerp(0, m_MaxSpeed, Mathf.Abs(forwardSpeed)); // Normalized speed factor

        // Reduce motor torque and steering at high speeds for better handling
        float currentMotorTorque = Mathf.Lerp(m_MotorTorque, 0, speedFactor);
        float currentSteerRange = Mathf.Lerp(m_SteeringRange, m_SteeringRangeAtMaxSpeed, speedFactor);

        // Determine if the player is accelerating or trying to reverse
        bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        // Iterate over the wheels
        for (int i = 0; i < m_Wheels.Length; i++)
        {
            if (m_Wheels[i].m_Steerable)
            {
                m_Wheels[i].m_WheelCollider.steerAngle = m_CurrentTurn * currentSteerRange;
            }

            if (isAccelerating)
            {
                // Apply torque to motorized wheels
                if (m_Wheels[i].m_Motorized)
                {
                    m_Wheels[i].m_WheelCollider.motorTorque = vInput * currentMotorTorque;
                }
                // Release brakes when accelerating
                m_Wheels[i].m_WheelCollider.brakeTorque = 0f;
            }
            else
            {
                // Apply brakes when reversing direction
                m_Wheels[i].m_WheelCollider.motorTorque = 0f;
                m_Wheels[i].m_WheelCollider.brakeTorque = Mathf.Abs(vInput) * m_BrakeTorque;
            }
        }

        // Reversing
        bool isReversing = Mathf.Sign(vInput) < 0.0f;
        m_ReverseSound.m_Playing = isReversing;
        
        float targetEmissive = isReversing ? m_ReverseLightEmissiveStrengths.y : m_ReverseLightEmissiveStrengths.x;
        m_CurrentLightEmissive = Mathf.Lerp(m_CurrentLightEmissive, targetEmissive, Time.fixedDeltaTime * m_ReverseLightLerp);
        m_ReverseLightMaterial.SetFloat("_EmissiveStrength", m_CurrentLightEmissive);

        float targetLight = isReversing ? m_ReverseLightStrengths.y : m_ReverseLightStrengths.x;
        m_CurrentLightStrength = Mathf.Lerp(m_CurrentLightStrength, targetLight, Time.fixedDeltaTime * m_ReverseLightLerp);
        for (int i = 0; i < m_ReverseLights.Length; i++)
        {
            m_ReverseLights[i].intensity = m_CurrentLightStrength;
        }

        // Exhaust
        bool shouldEmitExhaust = !Mathf.Approximately(vInput, 0.0f);
        m_EngineSound.m_Playing = shouldEmitExhaust;

        if (shouldEmitExhaust && !m_Exhaust.isEmitting)
        {
            m_Exhaust.Play();
        }
        else if (!shouldEmitExhaust && m_Exhaust.isEmitting)
        {
            m_Exhaust.Stop();
        }
    }

    /// <summary>
    /// Default Unity collision enter callback
    /// </summary>
    /// <param name="_collision">Data relevant to the collision</param>
    private void OnCollisionEnter(Collision _collision)
    {
        // Crash sounds
        if (!m_CrashSound.isPlaying)
        {
            m_CrashSound.pitch = Random.Range(m_CrashPitchRange.x, m_CrashPitchRange.y);
            m_CrashSound.volume = Random.Range(m_CrashVolumeRange.x, m_CrashVolumeRange.y);
            m_CrashSound.Play();
        }
    }
}