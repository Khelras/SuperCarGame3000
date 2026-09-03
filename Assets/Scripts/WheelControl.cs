using UnityEngine;

public class WheelControl : MonoBehaviour
{
    [Header("Internal Components")]
    public Transform m_WheelModel;
    public WheelCollider m_WheelCollider;

    [Header("Wheel Settings")]
    public bool m_Steerable;
    public bool m_Motorized;

    /// <summary>
    /// Editor-only function that is called when something changes, the scene starts, and 
    /// some other convenient locations/times
    /// </summary>
    private void OnValidate()
    {
        // Auto-attach wheel model
        if (!m_WheelModel)
        {
            m_WheelModel = transform.GetChild(0);
        }

        // Auto-attach wheel collider
        if (!m_WheelCollider)
        {
            m_WheelCollider = GetComponent<WheelCollider>();
        }
    }

    /// <summary>
    /// Our update loop from Unity, called as fast as possible 
    /// </summary>
    private void Update()
    {
        Vector3 wheelPosition;
        Quaternion wheelRotation;

        // Use world pose to update the model to be accurate
        m_WheelCollider.GetWorldPose(out wheelPosition, out wheelRotation);
        m_WheelModel.transform.position = wheelPosition;
        m_WheelModel.transform.rotation = wheelRotation;
    }
}