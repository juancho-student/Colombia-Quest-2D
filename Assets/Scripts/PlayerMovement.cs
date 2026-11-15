
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // PLAYER MOVEMENT
    private Rigidbody2D rb2D;

    public float moveSpeed = 5f;

    private float horizontalMovement;

    public float jumpHeight = 5f;

    public Transform groundCheck;

    public Vector2 groundCheckSize = new Vector2(0.25f, 0.25f);

    public LayerMask groundLayer;

    private bool isGrounded;

    private Animator animator;

    // COINS
    private int coins;

    public TMP_Text textCoins;

    // AUDIO
    public AudioSource audioSource;

    public AudioClip coinClip;

    public AudioClip barrelClip;

    // QUIZ SYSTEM
    [Header("Quiz System")]

    public LifeManager lifeManager;

    private ObstacleQuiz nearbyObstacle;

    private bool movementEnabled = true;

    // SPIKES CONTACT CONTROL
    private readonly HashSet<Collider2D> spikeContacts =
        new HashSet<Collider2D>();


    // INITIALIZATION
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        if (lifeManager == null)
        {
            lifeManager = FindAnyObjectByType<LifeManager>();
        }
    }


    // UPDATE
    private void Update()
    {
        CheckFlip();

        UpdateGroundedState();

        if (animator != null)
        {
            animator.SetFloat(
                "Horizontal",
                Mathf.Abs(horizontalMovement)
            );

            animator.SetBool("IsGrounded", isGrounded);
        }
    }


    // COLLISION AND TRIGGER DETECTION
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // COIN COLLECTION
        if (collision.CompareTag("Coin"))
        {
            if (audioSource != null && coinClip != null)
            {
                audioSource.PlayOneShot(coinClip);
            }

            Destroy(collision.gameObject);

            coins++;

            if (textCoins != null)
            {
                textCoins.text = coins.ToString();
            }
        }


        // SPIKES: LOSE ONE LIFE PER CONTACT
        if (collision.CompareTag("Spikes"))
        {
            if (spikeContacts.Add(collision))
            {
                if (lifeManager != null)
                {
                    lifeManager.LoseLife();
                }
                else
                {
                    Debug.LogWarning(
                        "LifeManager reference is missing!"
                    );
                }
            }
        }


        // BARREL INTERACTION
        if (collision.CompareTag("Barrel"))
        {
            if (audioSource != null && barrelClip != null)
            {
                audioSource.PlayOneShot(barrelClip);
            }

            Vector2 knockbackDirection =
                (rb2D.position -
                (Vector2)collision.transform.position).normalized;

            rb2D.linearVelocity = Vector2.zero;

            rb2D.AddForce(
                knockbackDirection * 3,
                ForceMode2D.Impulse
            );

            BoxCollider2D[] colliders =
                collision.gameObject.GetComponents<BoxCollider2D>();

            foreach (BoxCollider2D col in colliders)
            {
                col.enabled = false;
            }

            Animator barrelAnimator =
                collision.GetComponent<Animator>();

            if (barrelAnimator != null)
            {
                barrelAnimator.enabled = true;
            }

            Destroy(collision.gameObject, 0.5f);
        }
    }


    // COLLISION EXIT: RESET SPIKES CONTACT
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Spikes"))
        {
            spikeContacts.Remove(collision);
        }
    }


    // PHYSICS UPDATE
    void FixedUpdate()
    {
        if (!movementEnabled)
        {
            rb2D.linearVelocity = Vector2.zero;
            return;
        }

        rb2D.linearVelocity = new Vector2(
            horizontalMovement * moveSpeed,
            rb2D.linearVelocity.y
        );
    }


    // HORIZONTAL MOVEMENT - INPUT SYSTEM
    public void Move(InputAction.CallbackContext context)
    {
        if (!movementEnabled)
        {
            horizontalMovement = 0f;
            return;
        }

        horizontalMovement =
            context.ReadValue<Vector2>().x;
    }


    // JUMP - INPUT SYSTEM
    public void Jump(InputAction.CallbackContext context)
    {
        if (!movementEnabled ||
            !context.performed ||
            !isGrounded)
        {
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }

        rb2D.linearVelocity = new Vector2(
            rb2D.linearVelocity.x,
            jumpHeight
        );
    }


    // INTERACT WITH HISTORICAL OBSTACLES
    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed || !movementEnabled)
        {
            return;
        }

        if (nearbyObstacle != null)
        {
            nearbyObstacle.ActivateQuiz();
        }
    }


    // ENABLE OR DISABLE PLAYER MOVEMENT
    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;

        if (!movementEnabled)
        {
            horizontalMovement = 0f;

            if (rb2D != null)
            {
                rb2D.linearVelocity = Vector2.zero;
            }
        }
    }


    // REGISTER NEARBY OBSTACLE
    public void SetNearbyObstacle(ObstacleQuiz obstacle)
    {
        nearbyObstacle = obstacle;
    }


    // CLEAR NEARBY OBSTACLE
    public void ClearNearbyObstacle(ObstacleQuiz obstacle)
    {
        if (nearbyObstacle == obstacle)
        {
            nearbyObstacle = null;
        }
    }


    // GROUND DETECTION VISUALIZATION
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (groundCheck != null)
        {
            Gizmos.DrawWireCube(
                groundCheck.position,
                groundCheckSize
            );
        }
    }


    // CHECK IF PLAYER IS GROUNDED
    void UpdateGroundedState()
    {
        if (groundCheck == null)
        {
            return;
        }

        isGrounded = Physics2D.OverlapBox(
            groundCheck.position,
            groundCheckSize,
            0,
            groundLayer
        );
    }


    // CHANGE PLAYER FACING DIRECTION
    void CheckFlip()
    {
        if (horizontalMovement == 0)
        {
            return;
        }

        bool movingRight = horizontalMovement > 0;

        bool facingRight = transform.localScale.x > 0;

        if (movingRight != facingRight)
        {
            Vector3 scale = transform.localScale;

            scale.x *= -1;

            transform.localScale = scale;
        }
    }
}
