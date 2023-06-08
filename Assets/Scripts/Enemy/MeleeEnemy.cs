using UnityEngine;

public class MeleeEnemy : EnemyController
{
    [SerializeField] private float attackCooldown = 2f;
    private bool canAttack = true;

    [SerializeField] private GameObject attackPrefab;

    [SerializeField] private float minSpeed = 2.0f;
    private float distanceToPlayer;
    private float timeofLastAttack;


    void Start()
    {
        StartHealth();
    }

    // Update is called once per frame
    void Update()
    {


        distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= range && GameTimer.TimeMultiplier != GameTimer.STOPPED)
        {
            if (canAttack)
            {
                Attack();
            }
        }
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

    public override void Attack()
    {
        if (Time.time - timeofLastAttack < attackCooldown)
        {
            return;
        }
        timeofLastAttack = Time.time;
        GameObject hitbox = Instantiate(attackPrefab, transform.position, Quaternion.identity);
        AttackAngleVisualisation(player.transform.position, hitbox);
    }

    void AttackAngleVisualisation(Vector2 targetPos, GameObject objectToRotate)
    {
        float degreeToAdd = AdvancedMath.GetAngleBetweenPoints(targetPos, transform.position, Vector2.right + targetPos);

        objectToRotate.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90 + degreeToAdd));
    }

    private float CalculateHealth()
    {
        return getHealth() / getMaxHealth();
    }
}
