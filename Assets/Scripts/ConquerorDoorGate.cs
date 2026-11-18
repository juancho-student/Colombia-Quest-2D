using UnityEngine;

public class ConquerorDoorGate : MonoBehaviour
{
    [Header("Enemies Required to Unlock Door")]
    public ConquerorHealth[] conquerors;

    [Header("Door Reference")]
    public Collider2D doorBlocker;

    void Start()
    {
        if (doorBlocker == null || conquerors == null || conquerors.Length == 0)
        {
            Debug.LogError("Missing references in ConquerorDoorGate!");
            return;
        }

        doorBlocker.enabled = true;
    }

    void Update()
    {
        if (doorBlocker == null || conquerors == null)
            return;

        foreach (ConquerorHealth enemy in conquerors)
        {
            if (enemy == null || !enemy.IsDefeated)
                return;
        }

        if (doorBlocker.enabled)
        {
            doorBlocker.enabled = false;
            Debug.Log("All Conquerors defeated. Door unlocked!");
        }
    }
}
