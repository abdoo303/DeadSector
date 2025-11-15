using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    // Drag your PauseCanvas here
    public GameObject pauseCanvas;

    private bool isPaused = false;

    void Start()
    {
        // Make sure the pause menu is hidden and game is running
        isPaused = false;
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f; // Ensure time is running normally
    }

    void Update()
    {
        // Listen for the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseCanvas.SetActive(true);
        Time.timeScale = 0f; // This freezes the game!
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseCanvas.SetActive(false);
        Time.timeScale = 1f; // This un-freezes the game
    }

    public void ExitToMenu()
    {
        // IMPORTANT: Un-freeze time before leaving
        Time.timeScale = 1f;

        // Load the Main Menu (build index 0)
        SceneManager.LoadScene(0);
    }
}