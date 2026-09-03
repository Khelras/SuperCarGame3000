using UnityEngine;

public class CarCamera : MonoBehaviour
{
    [Header("External Components")]
    public Transform m_PlayerCar;

    [Header("General Flags")]
    public bool m_FollowCar = false;
    public bool m_LookAtCar = false;

    [Header("Follow Settings")]
    public float m_FollowDistance = 6.0f;
    public float m_FollowHeight = 2.0f;
    public Vector3 m_LookOffset = Vector3.zero;
    
    public bool m_UseCameraCollision = true;
    public LayerMask m_CameraCollisionLayerMask;
    public float m_CollisionAdjustment = 0.5f;

    public float m_RotationArrivalTime = 0.75f;
    public float m_MoveArrivalTime = 0.5f;

    public float m_HardLookAtSpeed = 8.0f;

    private Vector3 m_RotateVelocity = Vector3.zero;
    private Vector3 m_MoveVelocity = Vector3.zero;

    private Vector3 m_TargetPosition = Vector3.zero;
    private Vector3 m_TargetEulerAngles = Vector3.zero;

    /// <summary>
    /// Resets the camera back to a stable position & rotation
    /// </summary>
    public void ResetCamera()
    {
        transform.localEulerAngles = new Vector3(0.0f, m_PlayerCar.localEulerAngles.y, 0.0f);
        FollowCar();
        transform.position = m_TargetPosition;
        transform.localEulerAngles = m_TargetEulerAngles;
        if (m_LookAtCar)
        {
            transform.LookAt(m_PlayerCar.position + m_LookOffset);
        }
        m_RotateVelocity = Vector3.zero;
        m_MoveVelocity = Vector3.zero;
    }

    /// <summary>
    /// Smoothly moves our camera towards target position & rotation
    /// </summary>
    private void SmoothMoveTo()
    {
        Vector3 currentPos = Vector3.SmoothDamp(transform.position, m_TargetPosition, ref m_MoveVelocity, m_MoveArrivalTime, 9999.0f, Time.fixedDeltaTime);
        transform.position = currentPos;

        Vector3 currentRot = transform.localEulerAngles;
        currentRot.x = Mathf.SmoothDampAngle(currentRot.x, m_TargetEulerAngles.x, ref m_RotateVelocity.x, m_RotationArrivalTime, 9999.0f, Time.fixedDeltaTime);
        currentRot.y = Mathf.SmoothDampAngle(currentRot.y, m_TargetEulerAngles.y, ref m_RotateVelocity.y, m_RotationArrivalTime, 9999.0f, Time.fixedDeltaTime);
        currentRot.z = Mathf.SmoothDampAngle(currentRot.z, m_TargetEulerAngles.z, ref m_RotateVelocity.z, m_RotationArrivalTime, 9999.0f, Time.fixedDeltaTime);
        transform.localEulerAngles = currentRot;
    }

    /// <summary>
    /// Handles updating target position and angle based on the car 
    /// </summary>
    private void FollowCar()
    {
        float targetDistance = m_FollowDistance;
        if (m_UseCameraCollision)
        {
            RaycastHit hit;
            if (Physics.Raycast(m_PlayerCar.position, -transform.forward, out hit, m_FollowDistance, m_CameraCollisionLayerMask))
            {
                targetDistance = hit.distance - m_CollisionAdjustment;
            }
        }

        m_TargetPosition = m_PlayerCar.position;
        m_TargetPosition.y += m_FollowHeight;
        m_TargetPosition += Quaternion.Euler(0.0f, transform.eulerAngles.y, 0.0f) * new Vector3(0.0f, 0.0f, -targetDistance);
        
        Vector3 finalLookPos = m_PlayerCar.position + m_LookOffset;
        float y = transform.position.y - finalLookPos.y;
        float x = new Vector2(transform.position.x - finalLookPos.x, transform.position.z - finalLookPos.z).magnitude;
        float r = Mathf.Tan(y / x);
        m_TargetEulerAngles = new Vector3(Mathf.Rad2Deg * r, m_PlayerCar.eulerAngles.y, 0.0f);
    }

    /// <summary>
    /// Similar to Update, but called at a fixed time interval
    /// Use for physics-related calls
    /// </summary>
    private void FixedUpdate()
    {
        // Follows the car
        if (m_FollowCar)
        {
            FollowCar();
            SmoothMoveTo();
        }

        // Forces us to look at the car for slightly better feel
        if (m_LookAtCar)
        {
            Quaternion r = transform.rotation;
            transform.LookAt(m_PlayerCar.position + m_LookOffset);
            transform.rotation = Quaternion.Slerp(r, transform.rotation, m_HardLookAtSpeed * Time.fixedDeltaTime);
        }
    }
}
