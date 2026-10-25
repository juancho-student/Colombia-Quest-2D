using UnityEngine;
using UnityEngine.SceneManagement;

public class UpMenu : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Load the main menu scene.
            SceneManager.LoadScene("Menu");
        }
    }
}