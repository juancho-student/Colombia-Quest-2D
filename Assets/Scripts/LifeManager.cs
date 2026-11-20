using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LifeManager : MonoBehaviour
{
    [Header("Life Settings")]
    public int maxLives = 3;
    private int currentLives;

    [Header("UI Hearts")]
    public Image[] hearts;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip loseLifeSound;
    public AudioClip errorSound;
    public AudioClip defeatSound;

    private float nextDamageTime = 0f;
    public float damageCooldown = 1f;
    private bool restarting;

    public int CurrentLives => currentLives;

    void Start()
    {
        currentLives = maxLives;
        UpdateHearts();
    }

    public void LoseLife(AudioClip damageSound = null)
    {
        if (restarting)
            return;

        if (Time.unscaledTime < nextDamageTime)
            return;

        nextDamageTime = Time.unscaledTime + damageCooldown;

        currentLives--;

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        UpdateHearts();

        Debug.Log("Lives remaining: " + currentLives);

        if (currentLives <= 0)
        {
            restarting = true;
            StartCoroutine(RestartLevel());
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < currentLives;
        }
    }

    IEnumerator RestartLevel()
    {
        Time.timeScale = 0f;

        if (audioSource != null && defeatSound != null)
        {
            audioSource.PlayOneShot(defeatSound);

            yield return new WaitForSecondsRealtime(defeatSound.length);
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.5f);
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
