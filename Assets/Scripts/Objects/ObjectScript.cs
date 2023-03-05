using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

public class ObjectScript : ObjectHealth
{
    [Header("Rigidbody:")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float mass = 0f;
    [SerializeField] private float linearDrag = 0f;
    private float gravityScale;

    [Header("Damage:")]
    [SerializeField] private float factor = 1.0f;
    [SerializeField] private List<Sprite> states;
    [SerializeField] private float percent;
    private SpriteRenderer image;
    [SerializeField] private float minSpeed = 2.0f; 

    [Foldout("Info")]
    [DisableIf("true")] [SerializeField] private Vector2 velocity;

    void OnValidate()
    {
        if(GetComponent<Rigidbody2D>() != null && rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        else if (rb == null)
        {
            rb = this.gameObject.AddComponent<Rigidbody2D>();
        }

        if (this.gameObject.GetComponent<Collider2D>() == null)
        {
            Debug.LogError("ObjectScript -> NO COLLIDER ATTACHED");
        }

        if (states.Count == 0)
        {
            Debug.LogError("ObjectScript -> ADD AT LEAST ONE STATE");
        }
    }

    void Awake()
    {
        image = this.gameObject.GetComponent<SpriteRenderer>();
        image.sprite = states[0];
        rb.mass = mass;
        rb.drag = linearDrag;
        gravityScale = rb.gravityScale;
        this.StartHealth();
    }

    void Update()
    {
        this.changeSprite();

        percent = (this.getHealth() / this.getMaxHealth()) * 100;

        if (rb.velocity.x != 0.0f || rb.velocity.y != 0.0f)
        {
            velocity = rb.velocity;
        }

        rb.velocity *= GameTimer.timeMultiplayer;
        rb.gravityScale = gravityScale * GameTimer.timeMultiplayer;
    }

    public override void OnDead()
    {
        Destroy(gameObject);
    }

    public float getMass()
    {
        return mass;
    }

    public void setMass(float value)
    {
        if (value < 0f)
        {
            return;
        }

        mass = value;
    }

    public void setFactor(float value)
    {
        if (value < 0f)
        {
            return;
        }

        factor = value;
    }

    private void changeSprite()
    {
        if (this.getHealth() == this.getMaxHealth())
        {
            image.sprite = states[0];
        }
        else
        {
            image.sprite = states[states.Count-1-Mathf.FloorToInt((states.Count/this.getMaxHealth())*this.getHealth())];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 vel = ComputeIncidentVelocity(collision);

        float speed = Mathf.Sqrt(Mathf.Pow(vel.x, 2) + Mathf.Pow(vel.y, 2));

        if (speed > minSpeed)
        {
            float damage = speed * (mass / 10) * factor;

            this.TakeDamage(damage);

            IDamageTaker damageTaker;
            if (collision.collider.gameObject.TryGetComponent<IDamageTaker>(out damageTaker))
            {
                damageTaker.TakeDamage(damage);
            }
        }
    }
}
