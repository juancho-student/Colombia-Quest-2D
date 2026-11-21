using UnityEngine;

// Controls the conqueror's horizontal patrol movement between two predefined positions.
public class ConquerorPatrol : MonoBehaviour
{
    // Defines the configuration parameters for the patrol behavior.
    [Header("Patrol Settings")]

    // Reference to the Transform that defines the left boundary of the patrol area.
    public Transform leftPoint;

    // Reference to the Transform that defines the right boundary of the patrol area.
    public Transform rightPoint;

    // Defines the horizontal movement speed of the conqueror.
    public float moveSpeed = 2f;

    // Stores the Rigidbody2D component used to control the conqueror's physics-based movement.
    private Rigidbody2D rb;

    // Stores the SpriteRenderer component used to control the character's visual orientation.
    private SpriteRenderer spriteRenderer;

    // Defines the current horizontal movement direction: 1 for right and -1 for left.
    private int direction = 1;

    // Determines whether the patrol movement is currently active.
    private bool patrolEnabled = true;

    // Initializes the required components and validates the patrol boundary references.
    void Start()
    {
        // Retrieve the Rigidbody2D component attached to the same GameObject.
        rb = GetComponent<Rigidbody2D>();

        // Retrieve the SpriteRenderer component used to flip the character sprite.
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Validate that both patrol boundary references have been assigned.
        if (leftPoint == null || rightPoint == null)
        {
            // Display an error message if either patrol point is missing.
            Debug.LogError("Assign both patrol points to Conqueror!");

            // Disable this component to prevent invalid patrol execution.
            enabled = false;
            return;
        }

        // Set the initial movement direction towards the right boundary.
        direction = 1;

        // Synchronize the sprite orientation with the initial movement direction.
        UpdateDirection();
    }

    // Executes physics-based movement and patrol boundary checks at fixed time intervals.
    void FixedUpdate()
    {
        // Check whether the patrol behavior has been temporarily disabled.
        if (!patrolEnabled)
        {
            // Stop horizontal movement while preserving the current vertical velocity.
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            return;
        }

        // Check whether the conqueror is moving right and has reached the right patrol boundary.
        if (direction == 1 &&
            transform.position.x >= rightPoint.position.x)
        {
            // Reverse the movement direction towards the left boundary.
            direction = -1;

            // Update the sprite orientation to match the new direction.
            UpdateDirection();
        }
        // Check whether the conqueror is moving left and has reached the left patrol boundary.
        else if (direction == -1 &&
                 transform.position.x <= leftPoint.position.x)
        {
            // Reverse the movement direction towards the right boundary.
            direction = 1;

            // Update the sprite orientation to match the new direction.
            UpdateDirection();
        }

        // Apply horizontal velocity according to the current direction and movement speed.
        // Preserve the existing vertical velocity to maintain gravity and jump physics.
        rb.linearVelocity = new Vector2(
            direction * moveSpeed,
            rb.linearVelocity.y
        );
    }

    // Enables or disables the patrol behavior through an external method call.
    public void SetPatrolEnabled(bool enabled)
    {
        // Update the patrol state using the specified parameter.
        patrolEnabled = enabled;

        // Stop horizontal movement immediately when the patrol is disabled.
        if (!patrolEnabled && rb != null)
        {
            // Preserve vertical velocity while resetting horizontal velocity.
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
        }
    }

    // Updates the horizontal sprite orientation based on the current movement direction.
    void UpdateDirection()
    {
        // Flip the sprite horizontally when moving to the left.
        spriteRenderer.flipX = direction == -1;
    }

    // Executes when the patrol component is disabled or destroyed.
    void OnDisable()
    {
        // Verify that the Rigidbody2D reference is valid before modifying its velocity.
        if (rb != null)

            // Stop horizontal movement while preserving vertical velocity.
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }
}
