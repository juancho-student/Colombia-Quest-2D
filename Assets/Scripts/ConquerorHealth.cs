using UnityEngine;

public class ConquerorHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Health Bar")]
    public Transform barFill;

    private Vector3 originalScale;

    public bool IsDefeated { get; private set; }

    void Start()
    {
        currentHealth = maxHealth;

        if (barFill != null)
        {
            originalScale = barFill.localScale;
        }
        else
        {
            Debug.LogError("Assign BarFill to ConquerorHealth!");
        }
    }

    public void TakeDamage()
    {
        if (IsDefeated)
            return;

        currentHealth--;

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            DefeatConqueror();
        }
    }

    void UpdateHealthBar()
    {
        if (barFill == null)
            return;

        float healthPercentage = (float)currentHealth / maxHealth;

        barFill.localScale = new Vector3(
            originalScale.x * healthPercentage,
            originalScale.y,
            originalScale.z
        );
    }

    void DefeatConqueror()
    {
        IsDefeated = true;

        GetComponent<ConquerorPatrol>().enabled = false;

        gameObject.SetActive(false);
    }
}