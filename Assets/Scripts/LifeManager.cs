using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

// Manages the player's life system, damage handling, UI updates, audio feedback, and level restart.
public class LifeManager : MonoBehaviour
{
    // Defines the configuration parameters for the player's health system.
    [Header("Life Settings")]

    // Specifies the maximum number of lives available to the player.
    public int maxLives = 3;

    // Stores the player's current number of remaining lives.
    private int currentLives;

    // Defines the UI elements used to visually represent the player's remaining lives.
    [Header("UI Hearts")]

    // Array of Image components representing the player's lives through heart icons.
    public Image[] hearts;

    // Defines the audio resources used for damage and defeat feedback.
    [Header("Sound Effects")]

    // Reference to the AudioSource component responsible for playing sound effects.
    public AudioSource audioSource;

    // Audio clip played when the player loses a life.
    public AudioClip loseLifeSound;

    // Audio clip reserved for incorrect answers or other error feedback.
    public AudioClip errorSound;

    // Audio clip played when the player loses all available lives.
    public AudioClip defeatSound;

    // Stores the timestamp at which the next damage event can be registered.
    private float nextDamageTime = 0f;

    // Defines the minimum time interval between consecutive damage events.
    public float damageCooldown = 1f;

    // Prevents multiple level restart operations from being triggered simultaneously.
    private bool restarting;

    // Provides read-only external access to the player's current number of lives.
    public int CurrentLives => currentLives;

    // Initializes the player's lives and synchronizes the heart UI elements.
    void Start()
    {
        // Set the current number of lives to the configured maximum value.
        currentLives = maxLives;

        // Update the heart icons to reflect the initial health state.
        UpdateHearts();
    }

    // Removes one life from the player, handles audio feedback, and checks for defeat.
    public void LoseLife(AudioClip damageSound = null)
    {
        // Prevent additional damage processing while the level restart sequence is active.
        if (restarting)
            return;

        // Check whether the damage cooldown period has elapsed.
        // Unscaled time ensures that damage timing remains independent of Time.timeScale.
        if (Time.unscaledTime < nextDamageTime)
            return;

        // Set the timestamp for the next valid damage event.
        nextDamageTime = Time.unscaledTime + damageCooldown;

        // Decrease the player's current number of lives by one.
        currentLives--;

        // Play the specified damage sound if the required audio references are available.
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        // Refresh the heart UI to display the updated number of remaining lives.
        UpdateHearts();

        // Log the player's remaining lives in the Unity Console for debugging purposes.
        Debug.Log("Lives remaining: " + currentLives);

        // Check whether the player has exhausted all available lives.
        if (currentLives <= 0)
        {
            // Set the restart state to prevent duplicate restart requests.
            restarting = true;

            // Start the coroutine responsible for handling the level restart sequence.
            StartCoroutine(RestartLevel());
        }
    }

    // Synchronizes the visibility of the heart icons with the player's current health.
    void UpdateHearts()
    {
        // Iterate through all heart UI elements.
        for (int i = 0; i < hearts.Length; i++)
        {
            // Enable each heart if its index is lower than the current number of lives.
            // Disable the remaining hearts to represent lost lives.
            hearts[i].enabled = i < currentLives;
        }
    }

    // Handles the defeat sequence and reloads the current level after the audio feedback.
    IEnumerator RestartLevel()
    {
        // Pause all time-dependent gameplay operations.
        Time.timeScale = 0f;

        // Check whether the audio source and defeat sound are properly assigned.
        if (audioSource != null && defeatSound != null)
        {
            // Play the defeat sound effect without interrupting other audio playback.
            audioSource.PlayOneShot(defeatSound);

            // Wait for the defeat sound to finish using real time,
            // since the game time scale has been set to zero.
            yield return new WaitForSecondsRealtime(defeatSound.length);
        }
        else
        {
            // Apply a fallback delay if the defeat audio references are unavailable.
            yield return new WaitForSecondsRealtime(0.5f);
        }

        // Restore the normal time scale before reloading the scene.
        Time.timeScale = 1f;

        // Reload the currently active scene using its build index.
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}