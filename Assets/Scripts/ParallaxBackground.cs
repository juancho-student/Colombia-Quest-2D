using UnityEngine;

// Controls the parallax scrolling effect to create a sense of depth in the background.
public class ParallaxBackground : MonoBehaviour
{
    // Defines the configuration parameters for the parallax effect.
    [Header("Parallax Settings")]

    // Reference to the camera Transform used to track its movement.
    public Transform cameraTransform;

    // Controls the intensity of the parallax effect.
    // The Range attribute restricts the Inspector value between 0 and 1.
    [Range(0f, 1f)]
    public float parallaxEffect = 0.1f;

    // Stores the camera position from the previous frame to calculate its displacement.
    private Vector3 lastCameraPosition;

    // Initializes the camera reference and stores its initial position.
    void Start()
    {
        // Check whether a camera Transform has been assigned in the Inspector.
        if (cameraTransform == null)
        {
            // Automatically retrieve the main camera Transform using its MainCamera tag.
            cameraTransform = Camera.main.transform;
        }

        // Store the initial camera position as the reference point for movement calculations.
        lastCameraPosition = cameraTransform.position;
    }

    // Updates the background position after the camera movement has been processed.
    void LateUpdate()
    {
        // Exit the method if the camera reference is unavailable.
        if (cameraTransform == null)
            return;

        // Calculate the camera displacement by subtracting its previous position
        // from its current position.
        Vector3 cameraMovement =
            cameraTransform.position - lastCameraPosition;

        // Apply a proportional displacement to the background based on the camera movement.
        // The parallax factor determines how much the background moves relative to the camera.
        // The Z-axis remains unchanged to preserve the background's depth position.
        transform.position += new Vector3(
            cameraMovement.x * parallaxEffect,
            cameraMovement.y * parallaxEffect,
            0f
        );

        // Update the stored camera position to use as the reference in the next frame.
        lastCameraPosition = cameraTransform.position;
    }
}
