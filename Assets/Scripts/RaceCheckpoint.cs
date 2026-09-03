using UnityEngine;

public class RaceCheckpoint : MonoBehaviour
{
    [Header("External Components")]
    public RaceManager m_RaceManager;

    [Header("Internal Components")]
    public Transform m_LeftPillar;
    public Transform m_RightPillar;
    public Transform m_RaceLine;
    public Collider m_TriggerCollider;

    [Header("Checkpoint Settings")]
    public float m_CheckpointWidth;
    public bool m_IsMajorCheckpoint = false;

    private bool m_CheckpointHit = false;

    /// <summary>
    /// Simple helper function to update required elements based on width
    /// </summary>
    private void UpdateCheckpointStyle()
    {
        Vector3 v3;

        m_LeftPillar = transform.Find("PillarL");
        m_RightPillar = transform.Find("PillarR");
        m_RaceLine = transform.Find("Floor");
        m_TriggerCollider = transform.Find("Trigger").GetComponent<Collider>();
        
        v3 = m_LeftPillar.localPosition;
        v3.x = -m_CheckpointWidth * 0.5f;
        m_LeftPillar.localPosition = v3;

        v3 = m_RightPillar.localPosition;
        v3.x = m_CheckpointWidth * 0.5f;
        m_RightPillar.localPosition = v3;

        v3 = m_RaceLine.localScale;
        v3.x = m_CheckpointWidth;
        m_RaceLine.localScale = v3;

        v3 = m_TriggerCollider.transform.localScale;
        v3.x = m_CheckpointWidth;
        m_TriggerCollider.transform.localScale = v3;

        m_LeftPillar.gameObject.SetActive(m_IsMajorCheckpoint);
        m_RightPillar.gameObject.SetActive(m_IsMajorCheckpoint);
        m_RaceLine.gameObject.SetActive(m_IsMajorCheckpoint);
    }

    /// <summary>
    /// Editor-only function that is called when something changes, the scene starts, and 
    /// some other convenient locations/times
    /// </summary>
    private void OnValidate()
    {
        // Updates checkpoint styling dynamically so that we can adjust in editor easily
        UpdateCheckpointStyle();

        if (!m_RaceManager)
        {
            m_RaceManager = FindAnyObjectByType<RaceManager>();
        }
    }

    /// <summary>
    /// Called by our trigger when the car enters our collider
    /// Is only a valid "hit" when we are up to an appropriate part of the race
    /// </summary>
    public void AttemptToHitCheckpoint()
    {
        m_RaceManager.AttemptToHitCheckpoint(this);
    }

    /// <summary>
    /// Called when a checkpoint is successfully hit
    /// </summary>
    public void CompleteCheckpoint()
    {
        m_CheckpointHit = true;
    }

    /// <summary>
    /// Called when we reset the checkpoints on new lap, etc.
    /// </summary>
    public void ResetCheckpoint()
    {
        m_CheckpointHit = false;
    }
}
