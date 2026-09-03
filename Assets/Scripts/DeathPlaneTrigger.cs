using UnityEngine;

public class DeathPlaneTrigger : MonoBehaviour
{
    [Header("External Components")]
    public RaceManager m_RaceManager;

    /// <summary>
    /// Editor-only function that is called when something changes, the scene starts, and 
    /// some other convenient locations/times
    /// </summary>
    private void OnValidate()
    {
        // Find the race manager
        if (!m_RaceManager)
        {
            m_RaceManager = FindAnyObjectByType<RaceManager>();
        }
    }

    /// <summary>
    /// Unity call-back for trigger collider being entered
    /// </summary>
    /// <param name="_other">The other collider that triggered this collision</param>
    private void OnTriggerEnter(Collider _other)
    {
        // Ensures that we've collided with the player car
        if (_other.CompareTag("Car"))
        {
            m_RaceManager.BeginCarRespawn();
        }
    }
}
