using UnityEngine;

// Manages the conqueror's health system, damage handling, and defeat state.
public class ConquerorHealth : MonoBehaviour
{
    // Defines the maximum amount of damage the conqueror can receive before being defeated.
    [Header("Health Settings")]
    public int maxHealth = 3;

    // Stores the conqueror's current health points.
    private int currentHealth;

    // References the Transform component used to visually represent the health bar.
    [Header("Health Bar")]
    public Transform barFill;

    // Stores the original scale of the health bar to calculate proportional health changes.
    private Vector3 originalScale;

    // Indicates whether the conqueror has been defeated.
    // The property can only be modified internally and accessed externally.
    public bool IsDefeated { get; private set; }

    // Initializes the conqueror's health and stores the original health bar dimensions.
    void Start()
    {
        // Set the current health to the configured maximum health value.
        currentHealth = maxHealth;

        // Check whether the health bar reference has been assigned.
        if (barFill != null)
        {
            // Store the initial scale of the health bar for subsequent updates.
            originalScale = barFill.localScale;
        }
        else
        {
            // Display an error message if the health bar reference is missing.
            Debug.LogError("Assign BarFill to ConquerorHealth!");
        }
    }

    // Applies one damage point to the conqueror and checks its remaining health.
    public void TakeDamage()
    {
        // Prevent additional damage if the conqueror has already been defeated.
        if (IsDefeated)
            return;

        // Decrease the current health by one point.
        currentHealth--;

        // Refresh the visual representation of the health bar.
        UpdateHealthBar();

        // Check whether the conqueror has reached zero or negative health.
        if (currentHealth <= 0)
        {
            // Execute the defeat sequence when no health points remain.
            DefeatConqueror();
        }
    }

    // Updates the health bar scale according to the remaining health percentage.
    void UpdateHealthBar()
    {
        // Exit the method if the health bar reference is not assigned.
        if (barFill == null)
            return;

        // Calculate the remaining health as a percentage of the maximum health.
        float healthPercentage = (float)currentHealth / maxHealth;

        // Adjust the horizontal scale of the health bar while preserving its original height and depth.
        barFill.localScale = new Vector3(
            originalScale.x * healthPercentage,
            originalScale.y,
            originalScale.z
        );
    }

    // Executes the conqueror's defeat sequence and disables its patrol behavior.
    void DefeatConqueror()
    {
        // Update the defeat state to indicate that the conqueror has been eliminated.
        IsDefeated = true;

        // Disable the patrol component to stop the conqueror's movement behavior.
        GetComponent<ConquerorPatrol>().enabled = false;

        // Deactivate the entire GameObject, removing the conqueror from the active scene.
        gameObject.SetActive(false);
    }
}