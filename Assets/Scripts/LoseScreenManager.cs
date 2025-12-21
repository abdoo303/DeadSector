using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoseScreenManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button restartButton;
    public Button mainMenuButton;

    [Header("Scene Configuration")]
    [Tooltip("The Index of your Main Menu (usually 0)")]
    public int mainMenuIndex = 0;

    [Tooltip("The Index of your Game Scene (Level 1 & 2 combined)")]
    public int gameSceneIndex = 2;

    void Start()
    {
        // 1. Ensure Cursor is visible (Since we are in a menu)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. Setup Button Listeners
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(LoadMainMenu);
    }

    public void RestartGame()
    {
        // Reset time in case it was modified
        Time.timeScale = 1f;

        // Load the Game Scene (Index 1)
        // Your LevelManager in that scene will check PlayerPrefs and 
        // put you at the start of Level 1 or Level 2 automatically.
        SceneManager.LoadScene(gameSceneIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuIndex);
    }
}