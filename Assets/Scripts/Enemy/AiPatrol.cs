using UnityEngine;

public class AiPatrol : MonoBehaviour
{
    [SerializeField] private float walkSpead;
    private bool mustPatrol = true;
    private bool mustFlip;
    private float distanceToPlayer;

    [SerializeField] private EnemyController enemyRange;
    [SerializeField] private Transform player;
    [SerializeField] private Transform groundCheckerPosition;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [SerializeField] public new BoxCollider2D collider;

    private void OnValidate()
    {
        if (GetComponent<EnemyController>() != null && enemyRange == null)
        {
            enemyRange = GetComponent<EnemyController>();
        }
        else if (enemyRange == null)
        {
            Debug.LogError("AiPatrol -> No Enemy Range");
        }

        if (GetComponent<Rigidbody2D>() != null && rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        else if (enemyRange == null)
        {
            Debug.LogError("AiPatrol -> No RigidBody2D");
        }
    }

    private void FixedUpdate()
    {
        if (mustPatrol)
        {
            Patrol();
        }
        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= enemyRange.range) 
        {
            mustPatrol = false;
            if (player.position.x > transform.position.x && transform.localScale.x < 0 ||
            player.position.x < transform.position.x && transform.localScale.x > 0)
            {
                Flip();
            }
        }
        else
        {
            mustPatrol = true;
        }

        if (mustPatrol)
        {
            mustFlip = !Physics2D.OverlapCircle(groundCheckerPosition.position, 0.15f, groundLayer);
        }

        rb.velocity = rb.velocity * GameTimer.timeMultiplayer;
    }

    private void Patrol()
    {
        if (mustFlip || collider.IsTouchingLayers(wallLayer)) 
        {
            Flip();
        }
        rb.velocity = new Vector2(walkSpead, rb.velocity.y);
    }

    public void Flip()
    {
        mustPatrol = false;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        walkSpead *= -1;
        mustPatrol = true;
    }
}
