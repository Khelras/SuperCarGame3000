using UnityEngine;

public class RaceCheckpointTrigger : MonoBehaviour
{
    [Header("Internal Components")]
    public RaceCheckpoint m_AttachedCheckpoint;

    /// <summary>
    /// Unity call-back for trigger collider being entered
    /// </summary>
    /// <param name="_other">The other collider that triggered this collision</param>
    private void OnTriggerEnter(Collider _other)
    {
        // Ensures that we've collided with the player car
        if (_other.CompareTag("Car"))
        {
            m_AttachedCheckpoint.AttemptToHitCheckpoint();
        }
    }
}
