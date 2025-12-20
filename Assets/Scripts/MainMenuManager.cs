using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button continueButton;
    public int gameSceneIndex = 2; // The Build Index of your main game scene

    void Start()
    {
        // 1. GET SAVED DATA
        // If no data exists, default to 1.
        int savedProgress = PlayerPrefs.GetInt("SavedLevel", 1);

        // 2. CHECK CONDITION
        // Only enable Continue if we have beaten Level 1 (so saved level is 2)
        if (savedProgress >= 2)
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }
    }

    public void StartNewGame()
    {
        // 1. RESET PROGRESS to Level 1
        PlayerPrefs.SetInt("SavedLevel", 1);
        PlayerPrefs.Save();

        // 2. Load the game
        SceneManager.LoadScene(gameSceneIndex);
    }

    public void ContinueGame()
    {
        // 1. Load the game scene
        // Your LevelManager script inside this scene will read "SavedLevel = 2"
        // and instantly teleport you to the Boss.
        SceneManager.LoadScene(gameSceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}