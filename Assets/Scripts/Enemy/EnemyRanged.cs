using System;
using System.Collections;
using UnityEngine;

public class EnemyRanged : EnemyController
{
    [SerializeField] private SpriteRenderer moveSprite;
    [SerializeField] private SpriteRenderer attackSprite;


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
            if (!IsObjectBlockedByOtherObject(player.gameObject, viewRayBlockingLayers))
            {
                if (canShoot)
                {
                    Vector2 direction = (player.position - transform.position).normalized;
                    float angle = Vector2.Angle(Vector2.right, direction);
                    Debug.Log("Angle: " + angle);
                    if (angle <= 70 || angle >= 110)
                    {
                        Attack();
                    }
                }
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
        EnemyBullet enemyBullet = newBullet.GetComponent<EnemyBullet>();

        float t2 = (player.position.x - shootPosition.position.x) / (direction.x * shootSpeed);
        Vector2 v0 = ((Vector2)(player.position - shootPosition.position) + t2 * t2 * new Vector2(0f, 4.59f)) / t2;
        v0.y -= UnityEngine.Random.Range(-0.2f, 0.15f) * aim * shootSpeed;

        enemyBullet.velocity = v0;
        enemyBullet.GetComponent<Rigidbody2D>().velocity = v0;
        //enemyBullet.velocity = new Vector2(direction.x, direction.y - UnityEngine.Random.Range(-0.2f, 0.15f) * aim) * shootSpeed;
        AudioSource s = Instantiate(source, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<AudioSource>();
        s.clip = attackClip;
        s.volume = volume;
        s.mute = mute;
        s.Play();
        Destroy(s.gameObject, 2f);
        StartCoroutine(ChangeSpriteForAttack(0.2f));
        yield return new WaitForSeconds(attackRecharge);
        canShoot = true;
    }

    private IEnumerator ChangeSpriteForAttack(float waitTime)
    {
        yield return new WaitForSeconds(0.1f);
        moveSprite.enabled = false;
        attackSprite.enabled = true;
        yield return new WaitForSeconds(waitTime);
        /*transform.localScale = new Vector3(1f, 1f, 1f);
        transform.position = new Vector2(transform.position.x, transform.position.y - 1.0f);
        */
        StartCoroutine(RevertSpriteToNormal(0.1f));
    }

    private IEnumerator RevertSpriteToNormal(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        moveSprite.enabled = true;
        attackSprite.enabled = false;
       /* transform.localScale = new Vector3(4f, 4, 4f);
        transform.position = new Vector2(transform.position.x, transform.position.y + 1.0f);*/
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
