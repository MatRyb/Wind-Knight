using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPatrol : MonoBehaviour
{
    public float walkSpead;
    [HideInInspector]
    public bool mustPatrol;
    private bool mustFlip;
    private float distanceToPlayer;

    public EnemyController enemyRange;
    public Transform player;
    public Transform groundCheckerPosition;
    public Rigidbody2D rb;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    [SerializeField]
    public new BoxCollider2D collider;
    void Start()
    {
        mustPatrol = true;
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


    }

    private void Patrol()
    {
        if (mustFlip || collider.IsTouchingLayers(wallLayer)) 
        {
            Flip();
        }
        rb.velocity = new Vector3(walkSpead * Time.fixedDeltaTime, rb.velocity.y);
    }

    public void Flip()
    {
        mustPatrol = false;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        walkSpead *= -1;
        mustPatrol = true;
    }
}
