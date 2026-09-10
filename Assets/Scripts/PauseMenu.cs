using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private uint levelNumber = 0;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject calibrateRotationButton;
    
    [Header("Input Actions")]
    public InputActionAsset m_InputActions;

    public bool isPaused { get; private set; } = false;

    private void Start()
    {
        // Start Closed
        pauseMenuCanvas.SetActive(false);
    }

    private void Update()
    {
        if (m_InputActions.FindAction("Pause").WasPressedThisFrame())
        {
            if (isPaused) UnPauseGame();
            else PauseGame();
        }

        // Calibrate Rotation button is only relevant for Rotational Input mode
        if (InputModeManager.CurrentMode == SteeringInputMode.Rotational)
        {
            if (calibrateRotationButton != null) calibrateRotationButton.SetActive(true);
        }
        else
        {
            if (calibrateRotationButton != null) calibrateRotationButton.SetActive(false);
        }
    }

    public void PauseGame()
    {
        // Pause Game
        isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;

        // Unlock the Cursor
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;

        // Set the Level Text
        levelText.text = $"Level {levelNumber}";

        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(true);
    }

    public void UnPauseGame()
    {
        // Unpause Game
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Lock the Cursor
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
    }

    public void RestartLevel()
    {
        // Reloads the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitLevel()
    {
        // Unpause
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Go back to Main Menu
        SceneManager.LoadScene("Menu");
    }
}