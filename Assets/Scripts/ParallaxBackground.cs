
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Parallax Settings")]
    public Transform cameraTransform;

    [Range(0f, 1f)]
    public float parallaxEffect = 0.1f;

    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        if (cameraTransform == null)
            return;

        Vector3 cameraMovement =
            cameraTransform.position - lastCameraPosition;

        transform.position += new Vector3(
            cameraMovement.x * parallaxEffect,
            cameraMovement.y * parallaxEffect,
            0f
        );

        lastCameraPosition = cameraTransform.position;
    }
}

