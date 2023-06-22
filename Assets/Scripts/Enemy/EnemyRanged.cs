using System;
using System.Collections;
using UnityEngine;

public class EnemyRanged : EnemyController
{
    [SerializeField] private float shootSpeed = 2f;
    [Range(0,100f)][SerializeField] private float aim = 50.0f;
    private bool canShoot = true;

    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float minSpeed = 2.0f;

    [SerializeField] private float scaleFactor = 2.0f;

    private float distanceToPlayer;

    private void OnValidate()
    {
        if (rb == null && GetComponent<Rigidbody2D>() != null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        else if (rb == null && GetComponent<Rigidbody2D>() == null)
        {
            Debug.LogError(gameObject.name + " EnemyRanged -> No RigidBody2D");
        }
    }

    void Start()
    {
        aim = (100 - aim) / 100;
        StartHealth();
    }

    void Update()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if(distanceToPlayer <= range && GameTimer.TimeMultiplier != GameTimer.STOPPED)
        {
            if (canShoot && !IsObjectBlockedByOtherObject(player.gameObject, viewRayBlockingLayers))
            {    
                Attack();
            }
        }
    }

    public override void Attack()
    {
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        canShoot = false;
        yield return new WaitForSeconds(attackRecharge);
        Vector2 direction = (player.position - transform.position).normalized;
        GameObject newBullet = Instantiate(bullet, shootPosition.position, Quaternion.identity);
        newBullet.transform.localScale = transform.localScale / scaleFactor;
        newBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, direction.y - UnityEngine.Random.Range(-0.2f, 0.15f) * aim) * shootSpeed;
        yield return new WaitForSeconds(attackRecharge);
        canShoot = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 vel = ComputeIncidentVelocity(collision);

        float speed = Mathf.Sqrt(Mathf.Pow(vel.x, 2) + Mathf.Pow(vel.y, 2));

        if (speed > minSpeed)
        {
            float damage = speed * (rb.mass / 10);

            TakeDamage(damage);

            if (collision.collider.gameObject.TryGetComponent(out IDamageTaker damageTaker))
            {
                damageTaker.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
