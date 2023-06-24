using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ObjectScript : ObjectHealth
{
    [Header("Rigidbody:")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float mass = 0f;
    [SerializeField] private float linearDrag = 0f;
    private float gravityScale;

    [Header("Damage:")]
    [SerializeField] private float factor = 1.0f;
    private SpriteRenderer image;
    [SerializeField] private List<Sprite> states;
    [SerializeField] private float minSpeed = 2.0f;
    [SerializeField] private ParticleSystem destroyParticle;

    [Foldout("Info")]
    [DisableIf("true")] [SerializeField] private Vector2 velocity;
    [Foldout("Info")]
    [DisableIf("true")] [SerializeField] private float percent;
    [Foldout("Info")]
    [DisableIf("true")] [SerializeField] private float maxSpeed;
    [Foldout("Info")]
    [DisableIf("true")] [SerializeField] private float minDealtDamage;
    [Foldout("Info")]
    [DisableIf("true")] [SerializeField] private float maxDealtDamage;

    void OnValidate()
    {
        if(GetComponent<Rigidbody2D>() != null && rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        else if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        if (gameObject.GetComponent<Collider2D>() == null)
        {
            Debug.LogError("ObjectScript -> NO COLLIDER ATTACHED");
        }

        if (states.Count == 0)
        {
            Debug.LogError("ObjectScript -> ADD AT LEAST ONE STATE");
        }

        maxSpeed = 32.69955f;

        minDealtDamage = minSpeed * (mass / 10) * factor;

        maxDealtDamage = maxSpeed * (mass / 10) * factor;
    }

    void Awake()
    {
        image = gameObject.GetComponent<SpriteRenderer>();
        image.sprite = states[0];
        rb.mass = mass;
        rb.drag = linearDrag;
        gravityScale = rb.gravityScale;
        StartHealth();
    }

    void Update()
    {
        ChangeSprite();

        percent = (GetHealth() / GetMaxHealth()) * 100;

        if (rb.velocity.x != 0.0f || rb.velocity.y != 0.0f)
        {
            velocity = rb.velocity;
        }

        rb.velocity *= GameTimer.TimeMultiplier;
        rb.gravityScale = gravityScale * GameTimer.TimeMultiplier;
    }

    public override void OnDead()
    {
        ParticleSystem particle = Instantiate(destroyParticle, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
        particle.Play();
        Destroy(particle.gameObject, 3);
        Destroy(gameObject);
    }

    public float GetMass()
    {
        return mass;
    }

    public void SetMass(float value)
    {
        if (value < 0f)
        {
            return;
        }

        mass = value;
    }

    public void SetFactor(float value)
    {
        if (value < 0f)
        {
            return;
        }

        factor = value;
    }

    private void ChangeSprite()
    {
        if (GetHealth() == GetMaxHealth())
        {
            image.sprite = states[0];
        }
        else
        {
            image.sprite = states[states.Count-1-Mathf.FloorToInt((states.Count/GetMaxHealth())*GetHealth())];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 vel = ComputeIncidentVelocity(collision);

        float speed = Mathf.Sqrt(Mathf.Pow(vel.x, 2) + Mathf.Pow(vel.y, 2));

        if (speed > minSpeed)
        {
            float damage = speed * (mass / 10) * factor;

            TakeDamage(damage);

            if (collision.collider.gameObject.TryGetComponent<IDamageTaker>(out IDamageTaker damageTaker))
            {
                if (!collision.gameObject.CompareTag("Player"))
                {
                    damageTaker.TakeDamage(damage);
                }
            }
        }
    }
}
