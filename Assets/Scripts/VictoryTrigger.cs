using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VictoryTrigger : MonoBehaviour
{
    public ConquerorDoorGate doorGate;

    [Header("Victory Sound")]
    public AudioSource audioSource;
    public AudioClip levelPassSound;

    private bool victoryStarted;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || victoryStarted)
            return;

        if (doorGate != null && doorGate.AreAllConquerorsDefeated)
        {
            victoryStarted = true;

            PlayerPrefs.SetInt(
                "LastLevelBuildIndex",
                SceneManager.GetActiveScene().buildIndex
            );

            PlayerPrefs.Save();

            StartCoroutine(LoadVictoryScene());
        }
    }

    private IEnumerator LoadVictoryScene()
    {
        if (audioSource != null && levelPassSound != null)
        {
            audioSource.PlayOneShot(levelPassSound);

            yield return new WaitForSecondsRealtime(
                levelPassSound.length
            );
        }

        SceneManager.LoadScene("Victory");
    }
}
