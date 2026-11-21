using UnityEngine;

// Controls the door access based on the defeat status of the required enemies.
public class ConquerorDoorGate : MonoBehaviour
{
    // Defines the enemies that must be defeated to unlock the door.
    [Header("Required Enemies")]
    public ConquerorHealth[] conquerors;

    // Stores the collider responsible for blocking player access through the door.
    [Header("Door Configuration")]
    public Collider2D doorBlocker;

    // Tracks whether the door has already been unlocked.
    private bool doorUnlocked = false;

    // Checks whether all assigned conquerors have been defeated.
    // Returns false if the enemy array is empty or contains an invalid reference.
    public bool AreAllConquerorsDefeated
    {
        get
        {
            // Validate that the enemy array contains at least one element.
            if (conquerors == null || conquerors.Length == 0)
                return false;

            // Iterate through all assigned enemies to verify their defeat status.
            foreach (ConquerorHealth enemy in conquerors)
            {
                // Return false if an enemy reference is missing or the enemy is still alive.
                if (enemy == null || !enemy.IsDefeated)
                    return false;
            }

            // Return true only when all assigned enemies have been defeated.
            return true;
        }
    }

    // Initializes the door state and validates the required component references.
    void Start()
    {
        // Check whether the door collider and enemy references are properly assigned.
        if (doorBlocker == null || conquerors == null || conquerors.Length == 0)
        {
            // Display an error message in the Unity Console if references are missing.
            Debug.LogError("Missing references in ConquerorDoorGate!");
            return;
        }

        // Enable the door collider to prevent player access at the beginning of the level.
        doorBlocker.enabled = true;
    }

    // Continuously monitors the defeat status of all required enemies.
    void Update()
    {
        // Check whether all enemies have been defeated and the door is still locked.
        if (!doorUnlocked && AreAllConquerorsDefeated)
        {
            // Disable the door collider to allow the player to pass through.
            doorBlocker.enabled = false;

            // Update the door state to prevent repeated unlock operations.
            doorUnlocked = true;

            // Display a confirmation message in the Unity Console.
            Debug.Log("All Conquerors defeated. Door unlocked!");
        }
    }
}
