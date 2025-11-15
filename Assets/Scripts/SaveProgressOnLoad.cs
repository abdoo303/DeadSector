using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveProgressOnLoad : MonoBehaviour
{
    void Start()
    {
        // Get the build index of the current active scene
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

        // Save this as the "SavedLevel"
        // This will overwrite the old save (e.g., if they reach Level 3, it saves "3")
        PlayerPrefs.SetInt("SavedLevel", currentLevelIndex);

        // Optional: Force PlayerPrefs to save immediately
        PlayerPrefs.Save();
    }
}