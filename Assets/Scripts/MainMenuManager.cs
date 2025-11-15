using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes
using UnityEngine.UI; // Required for UI elements like Buttons

public class MainMenuManager : MonoBehaviour
{
    // Drag your "Continue" button here in the Inspector
    public Button continueButton;

    private int levelToLoad;

    void Start()
    {
        // Check if we have a saved level
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            // If we do, enable the continue button
            continueButton.interactable = true;

            // Get the saved level index
            levelToLoad = PlayerPrefs.GetInt("SavedLevel");
        }
        else
        {
            // If not, disable the continue button
            continueButton.interactable = false;
        }
    }

    public void StartNewGame()
    {
        // Delete any old save data
        PlayerPrefs.DeleteKey("SavedLevel");

        // Load "FirstLevel" (which is at build index 1)
        SceneManager.LoadScene(2);
    }

    public void ContinueGame()
    {
        // Load the scene we saved in PlayerPrefs
        // This will be 2 (SecondLevel) or higher
        SceneManager.LoadScene(levelToLoad);
    }

    public void QuitGame()
    {
        // Quits the application (only works in a built game, not the editor)
        Application.Quit();
    }
}