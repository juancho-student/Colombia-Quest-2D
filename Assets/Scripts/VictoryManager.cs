using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

// Manages victory screen navigation, level progression, and scene transitions.
public class VictoryManager : MonoBehaviour
{
    // Defines the PlayerPrefs key used to store the build index of the last completed level.
    private const string LastLevelKey = "LastLevelBuildIndex";

    // Loads the main menu scene when the player selects the menu option.
    public void GoToMenu()
    {
        // Load the scene identified by the name "Menu".
        SceneManager.LoadScene("Menu");
    }

    // Reloads the last recorded level when the player selects the restart option.
    public void RestartLevel()
    {
        // Retrieve the last completed level index from PlayerPrefs.
        // Use build index 1 as the default value if no previous progress has been saved.
        int lastLevel = PlayerPrefs.GetInt(LastLevelKey, 1);

        // Load the previously recorded level using its build index.
        SceneManager.LoadScene(lastLevel);
    }

    // Determines whether another playable level is available and loads the next scene.
    // Returns to the main menu if no additional playable level exists.
    public void ContinueLevel()
    {
        // Retrieve the build index of the last completed level.
        // Default to index 1 if no progress information is available.
        int lastLevel = PlayerPrefs.GetInt(LastLevelKey, 1);

        // Calculate the build index of the next level.
        int nextLevel = lastLevel + 1;

        // Verify that the calculated index is within the available Build Settings scenes.
        if (nextLevel < SceneManager.sceneCountInBuildSettings)
        {
            // Retrieve the full scene path associated with the next build index.
            string nextScenePath =
                SceneUtility.GetScenePathByBuildIndex(nextLevel);

            // Extract the scene name from its file path by removing the file extension.
            string nextSceneName =
                Path.GetFileNameWithoutExtension(nextScenePath);

            // Prevent loading non-playable scenes such as the victory screen or main menu.
            if (nextSceneName != "Victory" && nextSceneName != "Menu")
            {
                // Load the next playable level.
                SceneManager.LoadScene(nextLevel);
                return;
            }
        }

        // Return to the main menu when no additional playable level is available.
        GoToMenu();
    }
}

