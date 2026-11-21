using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// Detects level completion and manages the transition to the victory scene.
public class VictoryTrigger : MonoBehaviour
{
    // Reference to the ConquerorDoorGate component used to verify enemy defeat conditions.
    public ConquerorDoorGate doorGate;

    // Defines the audio resources used during the level completion sequence.
    [Header("Victory Sound")]

    // Reference to the AudioSource component responsible for playing the victory sound.
    public AudioSource audioSource;

    // Audio clip played when the player successfully completes the level.
    public AudioClip levelPassSound;

    // Prevents the victory sequence from being triggered multiple times.
    private bool victoryStarted;

    // Detects when another Collider2D enters the victory trigger area.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore collisions from objects that are not tagged as Player.
        // Also prevent repeated execution if the victory sequence has already started.
        if (!collision.CompareTag("Player") || victoryStarted)
            return;

        // Verify that the door gate reference is assigned
        // and all required conqueror enemies have been defeated.
        if (doorGate != null && doorGate.AreAllConquerorsDefeated)
        {
            // Set the victory state to prevent duplicate trigger activation.
            victoryStarted = true;

            // Store the build index of the currently active level in PlayerPrefs.
            // This value is used by VictoryManager to manage level progression.
            PlayerPrefs.SetInt(
                "LastLevelBuildIndex",
                SceneManager.GetActiveScene().buildIndex
            );

            // Save the updated progress data to persistent storage.
            PlayerPrefs.Save();

            // Start the coroutine responsible for playing the victory sound
            // and loading the victory scene.
            StartCoroutine(LoadVictoryScene());
        }
    }

    // Handles the victory audio playback and subsequent scene transition.
    private IEnumerator LoadVictoryScene()
    {
        // Check whether the audio source and victory sound references are available.
        if (audioSource != null && levelPassSound != null)
        {
            // Play the level completion sound effect.
            audioSource.PlayOneShot(levelPassSound);

            // Wait until the audio clip finishes playing.
            // Realtime waiting ensures that the delay is independent of Time.timeScale.
            yield return new WaitForSecondsRealtime(
                levelPassSound.length
            );
        }

        // Load the victory scene after the audio playback is completed.
        SceneManager.LoadScene("Victory");
    }
}

