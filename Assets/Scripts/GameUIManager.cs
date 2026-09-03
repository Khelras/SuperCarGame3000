using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("External Components")]
    public RaceManager m_RaceManager;
    public Rigidbody m_PlayerCar;

    [Header("Annoucements")]
    public float m_AnnoucementLifeLength = 0.9f;
    public TextMeshProUGUI m_AnnoucementText;
    public TextMeshProUGUI m_LapAnnoucementText;
    public TextMeshProUGUI m_ProgressAnnoucementText;

    [Header("Top-Half Components")]
    public TextMeshProUGUI m_LapsText;
    public TextMeshProUGUI m_LapsTimerText;
    public TextMeshProUGUI m_ProgressText;
    public TextMeshProUGUI m_TotalTimerText;

    [Header("Speed-O-Meter")]
    public TextMeshProUGUI m_SpeedText;
    public Transform m_NeedleAnchor;
    public int m_StartRotation;
    public int m_EndRotation;
    public int m_EndSpeed;

    private float m_AnnoucementTimer = 0.0f;
    private float m_LapAnnoucementTimer = 0.0f;
    private float m_ProgressAnnoucementTimer = 0.0f;

    public void Start()
    {
        m_AnnoucementText.text = "";
        m_LapAnnoucementText.text = "";
        m_ProgressAnnoucementText.text = "";
    }

    /// <summary>
    /// Our update loop from Unity, called as fast as possible 
    /// </summary>
    private void Update()
    {
        // Annoucement
        if (m_AnnoucementTimer > 0.0f)
        {
            m_AnnoucementTimer -= Time.deltaTime;

            // Timer Finished
            if (m_AnnoucementTimer <= 0.0f)
            {
                m_AnnoucementTimer = 0.0f;
                m_AnnoucementText.text = "";
            }
        }

        // Laps Annoucement
        if (m_LapAnnoucementTimer > 0.0f)
        {
            m_LapAnnoucementTimer -= Time.deltaTime;

            // Timer Finished
            if (m_LapAnnoucementTimer <= 0.0f)
            {
                m_LapAnnoucementTimer = 0.0f;
                m_LapAnnoucementText.text = "";
            }
        }

        // Progress Annoucement
        if (m_ProgressAnnoucementTimer > 0.0f)
        {
            m_ProgressAnnoucementTimer -= Time.deltaTime;

            // Timer Finished
            if (m_ProgressAnnoucementTimer <= 0.0f)
            {
                m_ProgressAnnoucementTimer = 0.0f;
                m_ProgressAnnoucementText.text = "";
            }
        }

        // Laps
        m_LapsText.text = $"{m_RaceManager.CurrentLap-1}/{m_RaceManager.m_MaxLaps}";
        m_LapsTimerText.text = $"{GetTimeFormatted(m_RaceManager.LapTime)}";

        // Progress
        float completedTrackLength = m_RaceManager.CompletedTrackLength;
        float fullTrackLength = m_RaceManager.FullTrackLength;
        int completedLaps = m_RaceManager.CurrentLap - 1;
        int progressPercentage = GetRaceProgressPercentage(completedLaps, m_RaceManager.m_MaxLaps, completedTrackLength, fullTrackLength);
        m_ProgressText.text = $"{progressPercentage}%";
        m_TotalTimerText.text = $"{GetTimeFormatted(m_RaceManager.RaceTime)}";

        // Speed-o-Meter
        float clampedSpeed = Mathf.Clamp(m_PlayerCar.linearVelocity.magnitude, 0f, m_EndSpeed);
        float rotationZ = GetNeedleRotation(clampedSpeed);
        m_NeedleAnchor.rotation = Quaternion.Euler(0f, 0f, rotationZ);
        m_SpeedText.text = $"{Mathf.FloorToInt(clampedSpeed)}";

        // Respawn Timer
        if (m_RaceManager.Respawning)
        {
            m_ProgressAnnoucementText.text = $"Died...";
            StartProgressAnnoucementTimer();
        }

        //// Reset text display
        //m_TextDisplay.text = "";


        //// Checkpoints + laps progress
        //m_TextDisplay.text += $"\nCheckpoint: {m_RaceManager.CurrentCheckpoint} / {m_RaceManager.m_Checkpoints.Length}";

        //// Best (saved to PlayerPrefs) lap + race time if available
        //m_TextDisplay.text += $"\nBest Lap Time: ";
        //if (!PlayerPrefs.HasKey($"BestLapTime_{SceneManager.GetActiveScene().name}")) 
        //{
        //    m_TextDisplay.text += "-";
        //}
        //else
        //{
        //    m_TextDisplay.text += $"{PlayerPrefs.GetFloat($"BestLapTime_{SceneManager.GetActiveScene().name}")} s";
        //}

        //m_TextDisplay.text += $"\nBest Race Time: ";
        //if (!PlayerPrefs.HasKey($"BestRaceTime_{SceneManager.GetActiveScene().name}")) 
        //{
        //    m_TextDisplay.text += $"-";
        //}
        //else
        //{
        //    m_TextDisplay.text += $"{PlayerPrefs.GetFloat($"BestRaceTime_{SceneManager.GetActiveScene().name}")} s";
        //}
    }

    public void StartAnnoucementTimer()
    {
        m_AnnoucementTimer = this.m_AnnoucementLifeLength;
    }

    public void StartLapAnnoucementTimer()
    {
        m_LapAnnoucementTimer = this.m_AnnoucementLifeLength;
    }

    public void StartProgressAnnoucementTimer()
    {
        m_ProgressAnnoucementTimer = this.m_AnnoucementLifeLength;
    }

    private string GetTimeFormatted(float timeInSeconds)
    {
        if (timeInSeconds <= 0f) return "00:00:00";

        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100f) % 100f); // two-digit hundredths

        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    private int GetRaceProgressPercentage(int completedLaps, int maxLaps, float completedTrackLength, float fullTrackLength)
    {
        float currentLapProgress = fullTrackLength > 0f ? completedTrackLength / fullTrackLength : 0f;
        float overallProgress = (completedLaps + currentLapProgress) / maxLaps;

        return Mathf.RoundToInt(Mathf.Clamp01(overallProgress) * 100f);
    }

    private float GetNeedleRotation(float speed)
    {
        float clampedSpeed = Mathf.Clamp(speed, 0f, m_EndSpeed);
        return Mathf.Lerp(m_StartRotation, m_EndRotation, clampedSpeed / m_EndSpeed);
    }
}
