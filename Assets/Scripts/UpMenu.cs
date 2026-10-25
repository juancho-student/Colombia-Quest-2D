using UnityEngine;
using UnityEngine.SceneManagement;

// Handles the transition back to the main menu scene.
public class UpMenu : MonoBehaviour
{
    // Called when another collider enters the trigger area.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks if the object entering the trigger is the player.
        if (collision.CompareTag("Player"))
        {
            // Load the main menu scene.
            SceneManager.LoadScene("Menu");
        }
    }
}