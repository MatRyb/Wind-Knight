using System;
using System.Collections;
using UnityEngine;

public class WaveEnemy : EnemyController
{
    private Rigidbody2D rb;
    private Rigidbody2D playerRB;
    private WaveGenerator wg;
    [SerializeField] private float escapeSpeedFactor = 50f;
    [SerializeField] private float minSpeed = 2f;
    [SerializeField] private float escapeRange;
    [SerializeField] private float rangeOffset = 50f;

    private bool canAttack = true;

    void Awake()
    {
        if (TryGetComponent(out rb))
        {
            rb.gravityScale = 0f;
        }
        else
        {
            Debug.LogError("Wave Enemy needs Rigidbody2D");
        }

        if (!TryGetComponent(out wg))
        {
            Debug.LogError("Wave Enemy needs WaveGenerator");
        }

        if (!player.gameObject.TryGetComponent(out playerRB))
        {
            Debug.LogError("Can't get  Rigidbody2D from player");
        }
    }

    void Start()
    {
        StartHealth();
    }

    private void FixedUpdate()
    {
        Vector3 dir = player.position - transform.position;

        if (isObjectBlockedByOtherObject(player.gameObject, viewRayBlockingLayers) || Mathf.Abs(Vector3.Distance(Vector3.zero, dir)) > range + rangeOffset)
        {
            rb.velocity = Vector2.zero;
            canAttack = true;
            return;
        }

        if (Mathf.Abs(Vector3.Distance(Vector3.zero, dir)) > escapeRange && Mathf.Abs(Vector3.Distance(Vector3.zero, dir)) <= range)
        {
            if (GameTimer.timeMultiplayer == 1f)
            {
                rb.velocity = Vector2.zero;

                float degreeToAdd = AdvancedMath.GetAngleBetweenPoints(transform.position, player.position, Vector2.left - new Vector2(player.position.x, player.position.y));

                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -180 + degreeToAdd));

                if (canAttack)
                {
                    canAttack = false;
                    Attack();
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        else if (Mathf.Abs(Vector3.Distance(Vector3.zero, dir)) <= escapeRange)
        {
            canAttack = false;
            if (GameTimer.timeMultiplayer == 1f)
            {
                Vector3 vel = - dir.normalized * escapeSpeedFactor * playerRB.velocity;
                rb.velocity = vel;

                if ((vel.x > 0 && transform.rotation.z % 360 != 0) || (vel.x < 0 && transform.rotation.z % 360 != 180))
                {
                    transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, (vel.x > 0 ? 0f : -180f), transform.rotation.w);
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    public override void Attack()
    {
        LocalTimersManager.CreateNewTimer(attackRecharge).DoAfter(() =>
        {
            wg.SpawnWave();
            canAttack = true;
        }).Start();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 vel = ComputeIncidentVelocity(collision);

        float speed = Mathf.Sqrt(Mathf.Pow(vel.x, 2) + Mathf.Pow(vel.y, 2));

        if (speed > minSpeed)
        {
            float damage = speed * (this.GetComponent<Rigidbody2D>().mass / 10);

            this.TakeDamage(damage);

            IDamageTaker damageTaker;
            if (collision.collider.gameObject.TryGetComponent(out damageTaker))
            {
                damageTaker.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, escapeRange);
    }
}
