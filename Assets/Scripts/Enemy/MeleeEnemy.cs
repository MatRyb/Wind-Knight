using System.Collections;
using UnityEngine;

public class MeleeEnemy : EnemyController
{
    private bool canAttack = true;
    private bool isDoneCooldown = true;

    [SerializeField] private SpriteRenderer currentSprite;

    [SerializeField] private PolygonCollider2D colliderNormalSprite;
    [SerializeField] private PolygonCollider2D colliderAttackSprite;

    //[SerializeField] private GameObject hpBar;
    [SerializeField] private GameObject attackPrefab;

    [SerializeField] private Sprite moveSprite;
    [SerializeField] private Sprite attackSprite;

    [SerializeField] private Transform tailPos;

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
            if (canAttack && isDoneCooldown)
            {
                Attack();
                StartCoroutine(ChangeSpriteForAttack(0.2f));
                isDoneCooldown = false;
                StartCoroutine(WaitCooldown(attackRecharge));
            }
        }
    }

    private IEnumerator WaitCooldown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isDoneCooldown = true;
    }

    private IEnumerator ChangeSpriteForAttack(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        currentSprite.sprite = attackSprite;
        colliderAttackSprite.enabled = true;
        colliderNormalSprite.enabled = false;
        //transform.localScale = new Vector3(2.2f, 2.2f, 2.2f);
        //transform.position = new Vector2(transform.position.x, transform.position.y - 1.0f);
        //hpBar.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
        //hpBar.transform.position = new Vector2(transform.position.x, transform.position.y + 4f);

        StartCoroutine(RevertSpriteToNormal(0.1f));
    }

    private IEnumerator RevertSpriteToNormal(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        currentSprite.sprite = moveSprite;
        colliderAttackSprite.enabled = false;
        colliderNormalSprite.enabled = true;
        //transform.localScale = new Vector3(4f, 4, 4f);
        //transform.position = new Vector2(transform.position.x, transform.position.y + 1.0f);
        //hpBar.transform.localScale = new Vector3(1f, 1f, 1f);
        attacking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 vel = ComputeIncidentVelocity(collision);

        float speed = Mathf.Sqrt(Mathf.Pow(vel.x, 2) + Mathf.Pow(vel.y, 2));

        if (speed > minSpeed)
        {
            float damage = speed * (GetComponent<Rigidbody2D>().mass / 10);

            TakeDamage(damage);

            if (collision.collider.gameObject.TryGetComponent(out IDamageTaker damageTaker))
            {
                damageTaker.TakeDamage(damage);
            }
        }
    }

    public override void Attack()
    {
        attacking = true;
        if (Time.time - timeofLastAttack < attackRecharge)
        {  
            return;

        }
        timeofLastAttack = Time.time;

        GameObject mainHitBox = Instantiate(attackPrefab, tailPos.position, Quaternion.identity);
        AttackAngleVisualisation(player.transform.position, mainHitBox);
        AudioSource s = Instantiate(source, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<AudioSource>();
        s.clip = attackClip;
        s.volume = volume;
        s.mute = mute;
        s.Play();
        Destroy(s.gameObject, 2f);
        canAttack = true;
    }

    void AttackAngleVisualisation(Vector2 targetPos, GameObject objectToRotate)
    {
        float degreeToAdd = AdvancedMath.GetAngleBetweenPoints(targetPos, transform.position, Vector2.right + targetPos);

        objectToRotate.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90 + degreeToAdd));
    }
}
