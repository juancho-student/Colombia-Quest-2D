using UnityEngine;
using UnityEngine.SceneManagement;

public class UpLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger detected: " + collision.name);

        if (collision.CompareTag("Player"))
        {
            int nextSceneIndex =
                SceneManager.GetActiveScene().buildIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                Debug.Log("Loading scene index: " + nextSceneIndex);

                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogError("There is no next scene in Build Profiles.");
            }
        }
    }
}