using UnityEngine;

public class FlyingEnemy : EnemyController
{
    [SerializeField] private PatrolFlying patrol;
    [SerializeField] private Rigidbody2D enemyRigidbody;
    [SerializeField] private GameObject body;
    [SerializeField] private float speed = 10f;

    [SerializeField] private bool attack = false;
    [SerializeField] private bool settingOff = false;
    [SerializeField] private float setOffRange = 3f;
    [SerializeField] private float setOffTime = 2f;
    [SerializeField] private float explosionPower = 2f;

    private void Start()
    {
        StartHealth();
    }

    private void FixedUpdate()
    {
        if (attack && !settingOff)
        {
            Vector3 dir = player.position - gameObject.transform.position;

            if (Mathf.Abs(Vector3.Distance(Vector3.zero, dir)) > setOffRange)
            {
                if (GameTimer.TimeMultiplier == GameTimer.PLAYING)
                {
                    Vector3 vel = dir.normalized * speed;
                    enemyRigidbody.velocity = vel;

                    if ((vel.x > 0 && body.transform.localScale.x < 0) || (vel.x < 0 && body.transform.localScale.x > 0))
                    {
                        body.transform.localScale = new Vector2(body.transform.localScale.x * -1, body.transform.localScale.y);
                    }
                }
                else
                {
                    enemyRigidbody.velocity = Vector3.zero;
                }
            }
            else if (!IsObjectBlockedByOtherObject(player.gameObject, viewRayBlockingLayers))
            {
                enemyRigidbody.velocity = Vector3.zero;
                enemyRigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
                Attack();
            }
        }   
    }

    public override void Attack()
    {
        settingOff = true;
        LocalTimersManager.CreateNewTimer(setOffTime).DoAfter(() =>
        {
            Explode();
        }).Start();
    }

    private void Explode()
    {
        Collider2D[] collidersInExplosion = Physics2D.OverlapCircleAll(transform.position, range);
        if (collidersInExplosion.Length > 0)
        {
            foreach (var collider in collidersInExplosion)
            {
                if (collider.TryGetComponent<IDamageTaker>(out IDamageTaker damageTaker))
                {
                    if ((Object)damageTaker == this)
                    {
                        continue;
                    }

                    float powerPercente = 1f - (Mathf.Abs(Vector2.Distance(collider.gameObject.transform.position, transform.position)) / range);
                    float damage = Mathf.Clamp01(powerPercente) * explosionPower;
                    damageTaker.TakeDamage(damage);
                }
            }
        }
        TakeDamage(GetMaxHealth());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerControler>(out PlayerControler _) && !attack)
        {
            if (!IsObjectBlockedByOtherObject(collision.gameObject, viewRayBlockingLayers))
            {
                player = collision.gameObject.transform;
                attack = true;
                enemyRigidbody.freezeRotation = true;
                patrol.StopPatrol();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Wall") 
            || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Object"))
        {
            Explode();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, setOffRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
