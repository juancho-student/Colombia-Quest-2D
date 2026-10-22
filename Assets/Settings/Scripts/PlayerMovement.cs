using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb2D;

    public float moveSpeed = 5f;

    float horizontalMovement;

    public float jumpHeight = 5f;

    public Transform groundCheck;

    public Vector2 groundCheckSize = new Vector2(0.25f, 0.25f);

    public LayerMask groundLayer;

    private bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckFlip();
        UpdateGroundedState();
    }
    void FixedUpdate()
    {
        rb2D.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb2D.linearVelocity.y);

    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.performed || !isGrounded)
            return;        
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpHeight);
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }

    void UpdateGroundedState()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer);
    }

    void CheckFlip()
    {
        if (horizontalMovement == 0)
            return;

        bool movingRight = horizontalMovement > 0;
        bool facingRight = transform.localScale.x > 0;

        if (movingRight != facingRight)
        {
            Vector3 Scale = transform.localScale;
            Scale.x *= -1;
            transform.localScale = Scale;

        }

    }
    
}
