using UnityEngine;

public class FlyingEnemy : EnemyController
{
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
            Vector3 dir = player.position - this.gameObject.transform.position;

            if (Mathf.Abs(Vector3.Distance(Vector3.zero, dir)) > setOffRange)
            {
                if (GameTimer.timeMultiplayer == 1f)
                {
                    Vector3 vel = dir.normalized * speed;
                    enemyRigidbody.velocity = vel;

                    if ((vel.x > 0 && transform.localScale.x < 0) || (vel.x < 0 && transform.localScale.x > 0))
                    {
                        body.transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    }
                }
                else
                {
                    enemyRigidbody.velocity = Vector3.zero;
                }
            }
            else
            {
                enemyRigidbody.velocity = Vector3.zero;
                Attack();
            }
        }   
    }

    public override void Attack()
    {
        Debug.Log("SETTING OFF");
        settingOff = true;
        LocalTimersManager.CreateNewTimer(setOffTime).DoAfter(() =>
        {
            Debug.Log("EXPLOSION");
            Collider2D[] collidersInExplosion = Physics2D.OverlapCircleAll(transform.position, range);
            if (collidersInExplosion.Length > 0)
            {
                foreach (var collider in collidersInExplosion)
                {
                    IDamageTaker damageTaker;
                    if (collider.TryGetComponent<IDamageTaker>(out damageTaker))
                    {
                        if ((Object)damageTaker == this)
                        {
                            continue;
                        }

                        float powerPercente = 1f - (Mathf.Abs(Vector2.Distance(collider.gameObject.transform.position, transform.position)) / range);
                        float damage = Mathf.Clamp01(powerPercente) * explosionPower;
                        damageTaker.TakeDamage(damage);

                        Debug.Log(damageTaker);
                        Debug.Log(powerPercente);
                        Debug.Log(damage);
                    }
                }
            }
            Destroy(this.gameObject);
        }).Start();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !attack)
        {
            /*RaycastHit2D hit = Physics2D.Raycast(transform.position, collision.transform.position - transform.position);
            if (hit.collider.tag == "Player")
            {
                player = collision.gameObject.transform;
                attack = true;
            }*/

            player = collision.gameObject.transform;
            attack = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, setOffRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
