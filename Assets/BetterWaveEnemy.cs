using System;
using System.Collections;
using UnityEngine;

public class BetterWaveEnemy : EnemyController
{
    private Rigidbody2D rb;
    private WaveGenerator wg;
    [SerializeField] private float escapeRange;
    [SerializeField] private float rangeOffset = 50f;
    [SerializeField] private float moveDist = 10f;

    [SerializeField] private LayerMask rayCastLayer;
    [SerializeField] private ParticleSystem portalOut;
    [SerializeField] private ParticleSystem portalIn;

    private bool canAttack = true;
    private bool moved = false;

    private float factorX;
    private float factorY;

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
    }

    void Start()
    {
        StartHealth();
        rb.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        Vector3 dir = player.position - transform.position;
        float distance = Mathf.Abs(Vector3.Distance(Vector3.zero, dir));

        if (isObjectBlockedByOtherObject(player.gameObject, viewRayBlockingLayers) || distance > range + rangeOffset)
        {
            canAttack = true;
            return;
        } 
        else if (distance > escapeRange && distance <= range)
        {
            if (GameTimer.TimeMultiplier == GameTimer.PLAYING)
            {
                float degreeToAdd = AdvancedMath.GetAngleBetweenPoints(transform.position, player.position, Vector3.right + transform.position);

                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, degreeToAdd));

                if (canAttack)
                {
                    canAttack = false;
                    Attack();
                }
            }
        }
        else if (distance <= escapeRange)
        {
            if (GameTimer.TimeMultiplier == GameTimer.PLAYING && !moved)
            {
                float degree = AdvancedMath.GetAngleBetweenPoints(transform.position, player.position, Vector3.right + transform.position);

                factorX = -Mathf.Cos(degree * Mathf.PI / 180);

                factorY = -Mathf.Sin(degree * Mathf.PI / 180);

                moved = true;
                if (Physics2D.Raycast(transform.position, new Vector2(factorX, factorY), moveDist, rayCastLayer))
                {
                    GameObject particleIn = Instantiate(portalIn.gameObject, transform.position, transform.rotation);
                    particleIn.GetComponent<ParticleSystem>().Play();
                    Destroy(particleIn, 2f);
                    transform.position += new Vector3(moveDist * -factorX, moveDist * -factorY, 0);
                    GameObject particleOut = Instantiate(portalOut.gameObject, transform.position, transform.rotation);
                    particleOut.GetComponent<ParticleSystem>().Play();
                    Destroy(particleOut, 2f);
                }
                else
                {
                    GameObject particleIn = Instantiate(portalIn.gameObject, transform.position, transform.rotation);
                    particleIn.GetComponent<ParticleSystem>().Play();
                    Destroy(particleIn, 2f);
                    transform.position += new Vector3(moveDist * factorX, moveDist * factorY, 0);
                    GameObject particleOut = Instantiate(portalOut.gameObject, transform.position, transform.rotation);
                    particleOut.GetComponent<ParticleSystem>().Play();
                    Destroy(particleOut, 2f);
                }
                moved = false;
            }
            else if (moved)
            {
                moved = false;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, escapeRange);
        Gizmos.DrawLine(transform.position, player.position);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + moveDist * factorX, transform.position.y + moveDist * factorY));
    }
}