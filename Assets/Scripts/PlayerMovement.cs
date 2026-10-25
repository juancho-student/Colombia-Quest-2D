using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // Stores the Rigidbody2D component used to control the player's physics.
    private Rigidbody2D rb2D;

    // Defines the player's horizontal movement speed.
    public float moveSpeed = 5f;

    // Stores the horizontal movement input from the player.
    float horizontalMovement;

    // Defines the upward velocity applied when the player jumps.
    public float jumpHeight = 5f;

    // Reference to the Transform used to check if the player is touching the ground.
    public Transform groundCheck;

    // Defines the size of the ground detection area.
    public Vector2 groundCheckSize = new Vector2(0.25f, 0.25f);

    // Specifies which layers are considered ground.
    public LayerMask groundLayer;

    // Indicates whether the player is currently touching the ground.
    private bool isGrounded;

    private Animator animator;

    private int coins;

    public TMP_Text textCoins;

    public AudioSource audioSource;

    public AudioClip coinClip;

    public AudioClip barrelClip;


    // Called once when the script is initialized.
    void Start()
    {
        // Gets the Rigidbody2D component attached to the player.
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Called once per frame.
    private void Update()
    {
        // Updates the player's facing direction based on horizontal movement.
        CheckFlip();

        // Checks whether the player is touching the ground.
        UpdateGroundedState();

        animator.SetFloat("Horizontal", Mathf.Abs(horizontalMovement));

        animator.SetBool("IsGrounded", isGrounded);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks if the player has collided with an object tagged as "Enemy".
        if (collision.transform.CompareTag("Coin"))
        {
            audioSource.PlayOneShot(coinClip); // Plays the coin collection sound effect.
            Destroy(collision.gameObject); // Destroys the coin object upon collision.
            coins++; // Increments the coin count.
            textCoins.text = coins.ToString();
        }

        if (collision.transform.CompareTag("Spikes"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reloads the current scene upon collision with spikes.
        }

        if (collision.transform.CompareTag("Barrel"))
        {
            audioSource.PlayOneShot(barrelClip); // Plays the barrel destruction sound effect.
            Vector2 knockbackDirection = (rb2D.position - (Vector2)collision.transform.position).normalized; // Calculates the direction of knockback.
            rb2D.linearVelocity= Vector2.zero; // Resets the player's velocity to zero.
            rb2D.AddForce(knockbackDirection * 3, ForceMode2D.Impulse); // Applies a knockback force to the player.

            BoxCollider2D[] colliders= collision.gameObject.GetComponents<BoxCollider2D>(); // Gets all BoxCollider2D components on the barrel object. 

            foreach (BoxCollider2D col in colliders)
            {
                col.enabled = false; // Disables each BoxCollider2D component on the barrel object.
            }

            collision.GetComponent<Animator>().enabled = true;
            Destroy(collision.gameObject, 0.5f); // Destroys the barrel object after a short delay.


        }
    }

    // Called at fixed intervals to handle physics calculations.
    void FixedUpdate()
    {
        // Applies horizontal movement while preserving the current vertical velocity.
        rb2D.linearVelocity = new Vector2(
            horizontalMovement * moveSpeed,
            rb2D.linearVelocity.y
        );
    }

    // Reads horizontal movement input from the Input System.
    public void Move(InputAction.CallbackContext context)
    {
        // Gets the horizontal value from the input action.
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    // Handles the player's jump input.
    public void Jump(InputAction.CallbackContext context)
    {
        // Stops the method if the jump input was not performed
        // or if the player is not touching the ground.
        if (!context.performed || !isGrounded)
            return;

        animator.SetTrigger("Jump");

        // Applies upward velocity to make the player jump.
        rb2D.linearVelocity = new Vector2(
            rb2D.linearVelocity.x,
            jumpHeight
        );
    }

    // Draws the ground detection area in the Unity Scene view.
    private void OnDrawGizmosSelected()
    {
        // Sets the Gizmo color to yellow for visibility.
        Gizmos.color = Color.yellow;

        // Draws a wireframe cube showing the ground detection area.
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }

    // Detects whether the player is touching a surface on the ground layer.
    void UpdateGroundedState()
    {
        // Checks for overlap between the detection area and the ground layer.
        isGrounded = Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0,
            groundLayer
        );
    }

    // Flips the player's horizontal scale to face the movement direction.
    void CheckFlip()
    {
        // Stops the method if there is no horizontal movement.
        if (horizontalMovement == 0)
            return;

        // Determines whether the player is moving to the right.
        bool movingRight = horizontalMovement > 0;

        // Determines whether the player is currently facing right.
        bool facingRight = transform.localScale.x > 0;

        // Flips the player if the movement direction differs from the facing direction.
        if (movingRight != facingRight)
        {
            // Stores the player's current scale.
            Vector3 Scale = transform.localScale;

            // Reverses the horizontal scale to change the facing direction.
            Scale.x *= -1;

            // Applies the updated scale to the player.
            transform.localScale = Scale;
        }
    }
}