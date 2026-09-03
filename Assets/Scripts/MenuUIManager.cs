using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuUIManager : MonoBehaviour
{
    [Header("External Components")]
    public InputActionAsset m_InputActions;
    
    [Header("Internal Components")]
    public GameObject m_MainMenu;
    public GameObject m_LevelSelectMenu;
    public TextMeshProUGUI m_LevelOneScoreDisplay;
    public TextMeshProUGUI m_LevelTwoScoreDisplay;
    public TextMeshProUGUI m_LevelThreeScoreDisplay;

    /// <summary>
    /// Placeholder function to close all menus
    /// </summary>
    private void CloseAllMenus()
    {
        m_MainMenu.SetActive(false);
        m_LevelSelectMenu.SetActive(false);
    }

    /// <summary>
    /// Placeholder function to open the main menu
    /// </summary>
    public void OnOpenMainMenu()
    {
        CloseAllMenus();
        m_MainMenu.SetActive(true);
    }

    /// <summary>
    /// Placeholder function to open the level select menu
    /// </summary>
    public void OnOpenLevelSelectMenu()
    {
        CloseAllMenus();
        DisplayScores();
        m_LevelSelectMenu.SetActive(true);
    }

    /// <summary>
    /// Placeholder function to play level 1
    /// </summary>
    public void OnPlayLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    /// <summary>
    /// Placeholder function to play level 2
    /// </summary>
    public void OnPlayLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    /// <summary>
    /// Placeholder function to play level 3
    /// </summary>
    public void OnPlayLevel3()
    {
        SceneManager.LoadScene("Level3");
    }

    /// <summary>
    /// Placeholder function to clear scores
    /// </summary>
    public void OnClearData()
    {
        PlayerPrefs.DeleteAll();
    }

    /// <summary>
    /// Placeholder function to quit
    /// </summary>
    public void OnExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Placeholder function to write our saved player scores to screen
    /// </summary>
    private void DisplayScores()
    {
        // Level One Score Display Text
        m_LevelOneScoreDisplay.text = "";
        m_LevelOneScoreDisplay.text += $"Best Lap Time: {GetScoreTimeFormatted($"BestLapTime_Level{1}")}";
        m_LevelOneScoreDisplay.text += $"\nBest Race Time: {GetScoreTimeFormatted($"BestRaceTime_Level{1}")}";

        // Level Two Score Display Text
        m_LevelTwoScoreDisplay.text = "";
        m_LevelTwoScoreDisplay.text += $"Best Lap Time: {GetScoreTimeFormatted($"BestLapTime_Level{2}")}";
        m_LevelTwoScoreDisplay.text += $"\nBest Race Time: {GetScoreTimeFormatted($"BestRaceTime_Level{2}")}";

        // Level Three Score Display Text
        m_LevelThreeScoreDisplay.text = "";
        m_LevelThreeScoreDisplay.text += $"Best Lap Time: {GetScoreTimeFormatted($"BestLapTime_Level{3}")}";
        m_LevelThreeScoreDisplay.text += $"\nBest Race Time: {GetScoreTimeFormatted($"BestRaceTime_Level{3}")}";
    }

    private string GetScoreTimeFormatted(string key)
    {
        float timeInSeconds = PlayerPrefs.GetFloat(key);

        if (timeInSeconds <= 0f) return "-";

        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100f) % 100f); // two-digit hundredths

        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    /// <summary>
    /// Called when this gameObject is first made active
    /// </summary>
    private void Start()
    {
        // Unlock mouse cursor because it can be left locked from the game scene!
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Start with the Main Menu
        OnOpenMainMenu();
    }

    /// <summary>
    /// Our update loop from Unity, called as fast as possible 
    /// </summary>
    private void Update()
    {
        // Use input to trigger exit as well
        if (m_InputActions.FindAction("Pause").WasPressedThisFrame())
        {
            OnExitGame();
        }
    }
}
