using UnityEngine;
using UnityEngine.SceneManagement;

// Manages the main menu operations, including game initialization and application termination.
public class MenuSystem : MonoBehaviour
{
    // Loads the next scene in the Build Settings to start the game.
    public void PlayGame()
    {
        // Retrieve the index of the currently active scene and load the subsequent scene.
        // This allows the game to transition from the main menu to the first playable level.
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex + 1
        );
    }

    // Terminates the application when the player selects the exit option.
    public void QuitGame()
    {
        // Display a confirmation message in the Unity Console for debugging purposes.
        Debug.Log("Exiting Game");

        // Request application termination.
        // This command takes effect in the final application build.
        Application.Quit();
    }
}
