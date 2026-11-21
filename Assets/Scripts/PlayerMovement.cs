using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// Controls the player's movement, physics interactions, animations, collectibles,
// damage system, quiz interactions, and collision responses.
public class PlayerMovement : MonoBehaviour
{
    // PLAYER MOVEMENT

    // Reference to the Rigidbody2D component used to handle physics-based movement.
    private Rigidbody2D rb2D;

    // Defines the player's horizontal movement speed.
    public float moveSpeed = 5f;

    // Stores the horizontal input value received from the Input System.
    private float horizontalMovement;

    // Defines the vertical force applied when the player jumps.
    public float jumpHeight = 5f;

    // Reference to the Transform used as the ground detection position.
    public Transform groundCheck;

    // Defines the dimensions of the overlap box used for ground detection.
    public Vector2 groundCheckSize = new Vector2(0.25f, 0.25f);

    // Specifies the physics layers considered valid ground surfaces.
    public LayerMask groundLayer;

    // Indicates whether the player is currently touching a valid ground surface.
    private bool isGrounded;

    // Reference to the Animator component responsible for controlling player animations.
    private Animator animator;


    // COINS

    // Stores the total number of coins collected by the player.
    private int coins;

    // Reference to the TextMeshPro UI component used to display the coin counter.
    public TMP_Text textCoins;


    // AUDIO

    // Reference to the AudioSource component used to play player sound effects.
    public AudioSource audioSource;

    // Audio clip played when the player collects a coin.
    public AudioClip coinClip;

    // Audio clip played when the player interacts with a barrel.
    public AudioClip barrelClip;


    // QUIZ SYSTEM

    // Defines the configuration and references required for quiz interactions.
    [Header("Quiz System")]

    // Reference to the LifeManager responsible for managing the player's remaining lives.
    public LifeManager lifeManager;

    // Stores the obstacle currently detected within the player's interaction range.
    private ObstacleQuiz nearbyObstacle;

    // Controls whether player movement and input interactions are enabled.
    private bool movementEnabled = true;

    // Indicates whether the player is currently experiencing knockback.
    private bool knockbackActive;


    // SPIKES CONTACT CONTROL

    // Stores the spike colliders currently overlapping the player's trigger area.
    // HashSet prevents duplicate damage registration from the same collider.
    private readonly HashSet<Collider2D> spikeContacts =
        new HashSet<Collider2D>();


    // INITIALIZATION

    // Initializes component references and retrieves the LifeManager instance.
    void Start()
    {
        // Retrieve the Rigidbody2D component attached to the player GameObject.
        rb2D = GetComponent<Rigidbody2D>();

        // Retrieve the Animator component used to control player animations.
        animator = GetComponent<Animator>();

        // Automatically locate an existing LifeManager if no reference was assigned.
        if (lifeManager == null)
        {
            lifeManager = FindAnyObjectByType<LifeManager>();
        }
    }


    // UPDATE

    // Processes frame-dependent player logic and updates animation parameters.
    private void Update()
    {
        // Update the player's horizontal facing direction.
        CheckFlip();

        // Refresh the player's grounded state using physics-based overlap detection.
        UpdateGroundedState();

        // Check whether the Animator component is available before updating parameters.
        if (animator != null)
        {
            // Update the horizontal movement parameter using the absolute input value.
            // This allows the Animator Controller to manage idle and running animations.
            animator.SetFloat(
                "Horizontal",
                Mathf.Abs(horizontalMovement)
            );

            // Synchronize the grounded state with the Animator Controller.
            animator.SetBool("IsGrounded", isGrounded);
        }
    }


    // COLLISION AND TRIGGER DETECTION

    // Detects interactions with collectible objects, environmental hazards, and obstacles.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // COIN COLLECTION

        // Check whether the detected object is tagged as a collectible coin.
        if (collision.CompareTag("Coin"))
        {
            // Play the coin collection sound if the audio references are available.
            if (audioSource != null && coinClip != null)
            {
                audioSource.PlayOneShot(coinClip);
            }

            // Remove the collected coin from the active scene.
            Destroy(collision.gameObject);

            // Increment the player's total coin count.
            coins++;

            // Update the coin counter UI if the TextMeshPro reference is assigned.
            if (textCoins != null)
            {
                textCoins.text = coins.ToString();
            }
        }


        // SPIKES: LOSE ONE LIFE PER CONTACT

        // Check whether the detected object is tagged as a spike hazard.
        if (collision.CompareTag("Spikes"))
        {
            // Register the collider only if it is not already tracked.
            // This prevents repeated damage from the same overlapping collider.
            if (spikeContacts.Add(collision))
            {
                // Check whether the LifeManager reference is available.
                if (lifeManager != null)
                {
                    // Remove one player life and play the configured damage sound.
                    lifeManager.LoseLife(lifeManager.loseLifeSound);
                }
                else
                {
                    // Display a warning if the LifeManager reference is unavailable.
                    Debug.LogWarning(
                        "LifeManager reference is missing!"
                    );
                }
            }
        }


        // BARREL INTERACTION

        // Check whether the detected object is tagged as a barrel.
        if (collision.CompareTag("Barrel"))
        {
            // Play the barrel interaction sound if the audio references are available.
            if (audioSource != null && barrelClip != null)
            {
                audioSource.PlayOneShot(barrelClip);
            }

            // Calculate the normalized direction from the barrel towards the player.
            // This vector determines the direction of the physical knockback force.
            Vector2 knockbackDirection =
                (rb2D.position -
                (Vector2)collision.transform.position).normalized;

            // Reset the player's current velocity before applying the impulse.
            rb2D.linearVelocity = Vector2.zero;

            // Apply an instantaneous physical force to push the player away from the barrel.
            rb2D.AddForce(
                knockbackDirection * 3,
                ForceMode2D.Impulse
            );

            // Retrieve all BoxCollider2D components attached to the barrel GameObject.
            BoxCollider2D[] colliders =
                collision.gameObject.GetComponents<BoxCollider2D>();

            // Iterate through the retrieved colliders and disable their collision detection.
            foreach (BoxCollider2D col in colliders)
            {
                col.enabled = false;
            }

            // Retrieve the Animator component attached to the barrel.
            Animator barrelAnimator =
                collision.GetComponent<Animator>();

            // Enable the barrel Animator if the component is available.
            if (barrelAnimator != null)
            {
                barrelAnimator.enabled = true;
            }

            // Destroy the barrel GameObject after a short delay to allow its animation to play.
            Destroy(collision.gameObject, 0.5f);
        }
    }


    // CONQUEROR COLLISION DETECTION

    // Detects physical collisions with conqueror enemies and activates their quiz interactions.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignore collision processing when player movement is disabled.
        if (!movementEnabled)
            return;

        // Retrieve the ConquerorQuiz component from the collided GameObject.
        ConquerorQuiz conquerorQuiz =
            collision.gameObject.GetComponent<ConquerorQuiz>();

        // Exit if the collided object does not contain a ConquerorQuiz component.
        if (conquerorQuiz == null)
            return;

        // Determine whether the player is positioned above the enemy
        // and moving downward or nearly stationary on the vertical axis.
        bool landingOnTop =
            transform.position.y > collision.transform.position.y &&
            rb2D.linearVelocity.y <= 0.1f;

        // Activate the conqueror quiz when the player lands on top of the enemy.
        if (landingOnTop)
        {
            conquerorQuiz.ActivateQuiz();
        }
    }


    // COLLISION EXIT: RESET SPIKES CONTACT

    // Detects when the player exits a trigger collider associated with a spike hazard.
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check whether the exited collider belongs to a spike hazard.
        if (collision.CompareTag("Spikes"))
        {
            // Remove the collider from the contact collection.
            // This allows a new damage event if the player touches the same spikes again.
            spikeContacts.Remove(collision);
        }
    }


    // PHYSICS UPDATE

    // Applies physics-based horizontal movement at fixed time intervals.
    void FixedUpdate()
    {
        // Suspend movement processing while a knockback effect is active.
        if (knockbackActive)
            return;

        // Check whether normal player movement has been disabled.
        if (!movementEnabled)
        {
            // Stop all player movement when movement controls are disabled.
            rb2D.linearVelocity = Vector2.zero;
            return;
        }

        // Apply horizontal velocity based on the current input and movement speed.
        // Preserve the existing vertical velocity to maintain gravity and jump physics.
        rb2D.linearVelocity = new Vector2(
            horizontalMovement * moveSpeed,
            rb2D.linearVelocity.y
        );
    }


    // HORIZONTAL MOVEMENT - INPUT SYSTEM

    // Processes horizontal movement input received through the Unity Input System.
    public void Move(InputAction.CallbackContext context)
    {
        // Prevent movement input from being processed while movement is disabled.
        if (!movementEnabled)
        {
            horizontalMovement = 0f;
            return;
        }

        // Read the horizontal axis value from the input action.
        // Values typically range from -1 (left) to 1 (right).
        horizontalMovement =
            context.ReadValue<Vector2>().x;
    }


    // JUMP - INPUT SYSTEM

    // Processes jump input and applies vertical force when the player is grounded.
    public void Jump(InputAction.CallbackContext context)
    {
        // Validate that movement is enabled, the jump action has been performed,
        // and the player is currently grounded.
        if (!movementEnabled ||
            !context.performed ||
            !isGrounded)
        {
            return;
        }

        // Trigger the jump animation if the Animator component is available.
        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }

        // Apply the jump velocity while preserving the current horizontal velocity.
        rb2D.linearVelocity = new Vector2(
            rb2D.linearVelocity.x,
            jumpHeight
        );
    }


    // INTERACT WITH HISTORICAL OBSTACLES

    // Processes interaction input to activate the quiz associated with a nearby obstacle.
    public void Interact(InputAction.CallbackContext context)
    {
        // Continue only if the interaction input has been performed
        // and player movement is currently enabled.
        if (!context.performed || !movementEnabled)
        {
            return;
        }

        // Check whether a valid nearby obstacle has been registered.
        if (nearbyObstacle != null)
        {
            // Activate the historical quiz associated with the nearby obstacle.
            nearbyObstacle.ActivateQuiz();
        }
    }


    // ENABLE OR DISABLE PLAYER MOVEMENT

    // Controls the availability of player movement and resets velocity when disabled.
    public void SetMovementEnabled(bool enabled)
    {
        // Update the player movement state.
        movementEnabled = enabled;

        // Stop horizontal input and reset velocity when movement is disabled.
        if (!movementEnabled)
        {
            horizontalMovement = 0f;

            // Verify that the Rigidbody2D reference is available before modifying velocity.
            if (rb2D != null)
            {
                rb2D.linearVelocity = Vector2.zero;
            }
        }
    }


    // REGISTER NEARBY OBSTACLE

    // Registers an obstacle detected within the player's interaction range.
    public void SetNearbyObstacle(ObstacleQuiz obstacle)
    {
        // Store the reference to the currently detected obstacle.
        nearbyObstacle = obstacle;
    }


    // CLEAR NEARBY OBSTACLE

    // Removes the reference to an obstacle when the player leaves its interaction range.
    public void ClearNearbyObstacle(ObstacleQuiz obstacle)
    {
        // Clear the reference only if the specified obstacle matches the registered obstacle.
        if (nearbyObstacle == obstacle)
        {
            nearbyObstacle = null;
        }
    }


    // GROUND DETECTION VISUALIZATION

    // Draws a visual representation of the ground detection area in the Scene view.
    private void OnDrawGizmosSelected()
    {
        // Set the Gizmos drawing color to yellow for visibility in the Scene view.
        Gizmos.color = Color.yellow;

        // Verify that the ground detection Transform has been assigned.
        if (groundCheck != null)
        {
            // Draw a wireframe box representing the ground detection area.
            Gizmos.DrawWireCube(
                groundCheck.position,
                groundCheckSize
            );
        }
    }


    // CHECK IF PLAYER IS GROUNDED

    // Determines whether the player is touching a valid ground surface.
    void UpdateGroundedState()
    {
        // Exit if the ground detection Transform has not been assigned.
        if (groundCheck == null)
        {
            return;
        }

        // Perform an overlap box query to detect colliders within the ground detection area.
        // The groundLayer mask restricts detection to designated ground layers.
        isGrounded = Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0,
            groundLayer
        );

        // Reset conqueror quiz activation when the player is grounded.
        if (isGrounded)
        {
            // Retrieve all active ConquerorQuiz components in the scene.
            ConquerorQuiz[] conquerors =
            FindObjectsByType<ConquerorQuiz>();

            // Iterate through all detected conquerors.
            foreach (ConquerorQuiz conqueror in conquerors)
            {
                // Re-enable quiz activation for each active conqueror.
                conqueror.ResetActivation();
            }
        }
    }


    // CHANGE PLAYER FACING DIRECTION

    // Updates the player's horizontal orientation according to the movement input.
    void CheckFlip()
    {
        // Skip orientation updates when there is no horizontal movement input.
        if (horizontalMovement == 0)
        {
            return;
        }

        // Determine whether the player is moving towards the right.
        bool movingRight = horizontalMovement > 0;

        // Determine the current facing direction using the horizontal scale.
        bool facingRight = transform.localScale.x > 0;

        // Check whether the current orientation differs from the movement direction.
        if (movingRight != facingRight)
        {
            // Retrieve the current local scale of the player Transform.
            Vector3 scale = transform.localScale;

            // Invert the horizontal scale to flip the player sprite.
            scale.x *= -1;

            // Apply the updated scale to the player Transform.
            transform.localScale = scale;
        }
    }


    // CONQUEROR KNOCKBACK

    // Applies an external impulse to the player after a conqueror quiz interaction.
    public void ApplyConquerorKnockback(Vector2 force)
    {
        // Activate the knockback state to temporarily suspend normal movement processing.
        knockbackActive = true;

        // Clear horizontal input to prevent movement input from interfering with the impulse.
        horizontalMovement = 0f;

        // Reset the current Rigidbody2D velocity before applying the knockback force.
        rb2D.linearVelocity = Vector2.zero;

        // Apply the specified impulse force to the player.
        rb2D.AddForce(force, ForceMode2D.Impulse);

        // Start the coroutine responsible for ending the knockback state.
        StartCoroutine(EndKnockback());
    }


    // END KNOCKBACK

    // Controls the duration of the knockback state before restoring normal movement processing.
    private System.Collections.IEnumerator EndKnockback()
    {
        // Wait for 0.5 seconds before disabling the knockback state.
        yield return new WaitForSeconds(0.5f);

        // Restore normal physics-based movement processing.
        knockbackActive = false;
    }
}
