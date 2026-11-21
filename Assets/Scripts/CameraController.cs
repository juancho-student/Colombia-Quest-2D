using UnityEngine;

// Controls the camera movement and smoothly follows the assigned target.
public class CameraController : MonoBehaviour
{
    // Defines the target Transform that the camera will follow.
    public Transform objetive;

    // Controls the interpolation speed used to smooth camera movement.
    public float cameraVelocity = 0.025f;

    // Defines the positional offset applied relative to the target's position.
    public Vector3 displacement;

    // Executes after the Update method to ensure the camera follows the target's latest position.
    private void LateUpdate()
    {
        // Calculate the desired camera position by adding the offset to the target's position.
        Vector3 desiredPosition = objetive.position + displacement;

        // Interpolate between the current camera position and the desired position.
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            cameraVelocity
        );

        // Apply the interpolated position to the camera Transform.
        transform.position = smoothedPosition;
    }
}