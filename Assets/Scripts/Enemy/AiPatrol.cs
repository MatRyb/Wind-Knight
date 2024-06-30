using System.Collections;
using UnityEngine;

public class AiPatrol : MonoBehaviour
{
    [SerializeField] private float walkSpead;
    [SerializeField] private float maxFallingSpeed;
    [SerializeField] private float gravityScale;
    private bool mustPatrol = true;
    private bool mustFlip;
    private bool fliped = true;
    private float distanceToPlayer;

    [SerializeField] private EnemyController enemyController;
    [SerializeField] private Transform player;
    [SerializeField] private Transform frontGroundCheckerPosition;
    [SerializeField] private Transform backGroundCheckerPosition;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask viewBlockingLayers;

    [SerializeField] public bool flipCollider = false;
    [SerializeField] public new Collider2D collider;

    [SerializeField] private GameObject body;

    private void OnValidate()
    {
        if (GetComponent<EnemyController>() != null && enemyController == null)
        {
            enemyController = GetComponent<EnemyController>();
        }
        else if (enemyController == null)
        {
            Debug.LogError(gameObject.name + " AiPatrol -> No Enemy Controller");
        }

        if (GetComponent<Rigidbody2D>() != null && rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        else if (rb == null && GetComponent<Rigidbody2D>() == null)
        {
            Debug.LogError(gameObject.name + " AiPatrol -> No RigidBody2D");
        }
    }

    private void Awake()
    {
        if (player == null)
        {
            if (FindObjectOfType<PlayerControler>() != null)
                player = FindObjectOfType<PlayerControler>().transform;
        }

        gravityScale = rb.gravityScale;
    }

    private void FixedUpdate()
    {
        if (rb.velocity.y < maxFallingSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, maxFallingSpeed);
        }

        if (mustPatrol)
        {
            Patrol();
        }

        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= enemyController.range && !enemyController.IsObjectBlockedByOtherObject(player.gameObject, viewBlockingLayers) || enemyController.attacking) 
        {
            mustPatrol = false;
            if (player.position.x > body.transform.position.x && body.transform.localScale.x < 0 ||
            player.position.x < body.transform.position.x && body.transform.localScale.x > 0)
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
            ContactFilter2D contactFilter2D = new();
            contactFilter2D.SetLayerMask(groundLayer);

            Collider2D[] colliders = new Collider2D[10];

            int front = Physics2D.OverlapCircle(frontGroundCheckerPosition.position, 0.15f, contactFilter2D, colliders);
            int back = Physics2D.OverlapCircle(backGroundCheckerPosition.position, 0.15f, contactFilter2D, colliders);

            mustFlip = (front == 0 && back != 0);
        }

        rb.velocity *= GameTimer.TimeMultiplier;
        rb.gravityScale = gravityScale * GameTimer.TimeMultiplier;
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Object") || collision.gameObject.CompareTag("Enemy")) && fliped)
        {
            Flip();
        }
    }
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("Object") || collision.gameObject.CompareTag("Enemy")) && fliped)
        {
            Flip();
        }
    }

    private void Patrol()
    {
        if ((mustFlip || collider.IsTouchingLayers(wallLayer)) && fliped) 
        {
            Flip();
        }
        rb.velocity = new Vector2(walkSpead, rb.velocity.y);
    }

    public void Flip()
    {
        fliped = false;
        mustPatrol = false;
        body.transform.localScale = new Vector2(body.transform.localScale.x * -1, body.transform.localScale.y);
        walkSpead *= -1;
        mustPatrol = true;

        if (flipCollider)
        {
            collider.offset = new Vector2(-collider.offset.x, collider.offset.y);
        }

        StartCoroutine(Flipping(0.5f));
    }
    private IEnumerator Flipping(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        fliped = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(frontGroundCheckerPosition.position, 0.15f);
        Gizmos.DrawWireSphere(backGroundCheckerPosition.position, 0.15f);
    }
}
