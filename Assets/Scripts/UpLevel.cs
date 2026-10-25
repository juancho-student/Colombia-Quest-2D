using UnityEngine;
using UnityEngine.SceneManagement;

// Handles the transition to the next level when the player
// enters the trigger area.
public class UpLevel : MonoBehaviour
{
    // Called when another collider enters the trigger.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Displays the name of the object that entered the trigger.
        Debug.Log("Trigger detected: " + collision.name);

        // Checks if the object entering the trigger is the player.
        if (collision.CompareTag("Player"))
        {
            // Calculates the index of the next scene.
            int nextSceneIndex =
                SceneManager.GetActiveScene().buildIndex + 1;

            // Verifies that a next scene exists in the Build Settings.
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                // Displays the scene index that will be loaded.
                Debug.Log("Loading scene index: " + nextSceneIndex);

                // Loads the next scene.
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                // Displays an error if there is no next scene available.
                Debug.LogError("There is no next scene in Build Profiles.");
            }
        }
    }
}