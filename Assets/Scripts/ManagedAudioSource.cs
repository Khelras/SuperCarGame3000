using UnityEngine;

public class ManagedAudioSource : MonoBehaviour
{
    [Header("Internal Components")]
    public AudioSource m_ManagedSource;
    
    [Header("Audio Settings")]
    public bool m_Playing = false;

    public float m_FullVolume = 1.0f;

    public float m_TurnOnLerpSpeed = 2.0f;
    public float m_TurnOffLerpSpeed = 8.0f;
    private float m_CurrentVolume = 0.0f;

    public Vector2 m_PitchRange = Vector2.one;
    private bool m_HasBeenSetToPlay = false;

    /// <summary>
    /// Editor-only function that is called when something changes, the scene starts, and 
    /// some other convenient locations/times
    /// </summary>
    private void OnValidate()
    {
        // Auto-attach our audio source
        if (!m_ManagedSource)
        {
            m_ManagedSource = GetComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Similar to Update, but called at a fixed time interval
    /// Use for physics-related calls
    /// </summary>
    private void FixedUpdate()
    {
        // Lerp our current volume
        m_CurrentVolume = Mathf.Lerp(m_CurrentVolume, m_Playing ? m_FullVolume : 0.0f, (m_Playing ? m_TurnOnLerpSpeed : m_TurnOffLerpSpeed) * Time.fixedDeltaTime);
        
        // Stop and play audio source as required
        if (m_CurrentVolume < 0.01f)
        {
            if (m_ManagedSource.isPlaying)
            {
                m_ManagedSource.Stop();
            }
            m_HasBeenSetToPlay = false; // makes sure single play doesn't play again
        }
        if (m_CurrentVolume > 0.01f && !m_ManagedSource.isPlaying && !m_HasBeenSetToPlay)
        {
            m_ManagedSource.Play();
            m_HasBeenSetToPlay = true; // makes sure single play doesn't play again
        }

        // Adjust pitch based on volume
        float invLerp = Mathf.InverseLerp(0.0f, m_FullVolume, m_CurrentVolume);
        m_ManagedSource.volume = m_CurrentVolume;
        m_ManagedSource.pitch = Mathf.Lerp(m_PitchRange.x, m_PitchRange.y, invLerp);
    }
}
