
using UnityEngine;

public class ConquerorPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform leftPoint;
    public Transform rightPoint;
    public float moveSpeed = 2f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private int direction = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (leftPoint == null || rightPoint == null)
        {
            Debug.LogError("Assign both patrol points to Conqueror!");
            enabled = false;
            return;
        }

        direction = 1;
        UpdateDirection();
    }

    void FixedUpdate()
    {
        if (direction == 1 &&
            transform.position.x >= rightPoint.position.x)
        {
            direction = -1;
            UpdateDirection();
        }
        else if (direction == -1 &&
                 transform.position.x <= leftPoint.position.x)
        {
            direction = 1;
            UpdateDirection();
        }

        rb.linearVelocity = new Vector2(
            direction * moveSpeed,
            rb.linearVelocity.y
        );
    }

    void UpdateDirection()
    {
        spriteRenderer.flipX = direction == -1;
    }

    void OnDisable()
    {
        if (rb != null)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }
}
