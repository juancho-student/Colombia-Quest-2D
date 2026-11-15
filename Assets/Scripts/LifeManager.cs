
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{
    [Header("Life Settings")]
    public int maxLives = 3;
    private int currentLives;

    [Header("UI Hearts")]
    public Image[] hearts;

    private float nextDamageTime = 0f;
    public float damageCooldown = 1f;

    public int CurrentLives => currentLives;

    void Start()
    {
        currentLives = maxLives;
        UpdateHearts();
    }

    public void LoseLife()
    {
        // Prevent multiple life losses in a short period.
        if (Time.unscaledTime < nextDamageTime)
            return;

        nextDamageTime = Time.unscaledTime + damageCooldown;

        currentLives--;

        UpdateHearts();

        Debug.Log("Lives remaining: " + currentLives);

        if (currentLives <= 0)
        {
            RestartLevel();
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < currentLives;
        }
    }

    void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}

