using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform objetive; // The target the camera will follow
    public float cameraVelocity = 0.025f; // The speed at which the camera will follow the target

    public Vector3 displacement;

    private void LateUpdate()
    {
        // Calculate the desired position based on the target's position and the displacement
        Vector3 desiredPosition = objetive.position + displacement;
        // Smoothly interpolate the camera's position towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, cameraVelocity);

        transform.position = smoothedPosition; // Update the camera's position
    }
}
