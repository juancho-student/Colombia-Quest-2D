using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryTrigger : MonoBehaviour
{
    public ConquerorDoorGate doorGate;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (doorGate != null && doorGate.AreAllConquerorsDefeated)
        {
            PlayerPrefs.SetInt(
                "LastLevelBuildIndex",
                SceneManager.GetActiveScene().buildIndex
            );

            PlayerPrefs.Save();

            SceneManager.LoadScene("Victory");
        }
    }
}
