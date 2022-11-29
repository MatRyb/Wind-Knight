using UnityEngine;
using NaughtyAttributes;

public class ObjectScript : ObjectHealth
{
    [Header("Rigidbody:")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float mass = 0f;
    [SerializeField] private float linearDrag = 0f;

    [Header("Damage:")]
    [SerializeField] private float factor = 1.0f;

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
    }

    void Awake()
    {
        rb.mass = mass;
        rb.drag = linearDrag;
        this.StartHealth();
    }

    void Update()
    {
        if (rb.velocity.x != 0.0f || rb.velocity.y != 0.0f)
        {
            velocity = rb.velocity;
        }
    }

    public override void OnDead()
    {
        Destroy(gameObject);
        Debug.Log("Object Dead");
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 vel = ComputeIncidentVelocity(collision);

        float speed = Mathf.Sqrt(Mathf.Pow(vel.x, 2) + Mathf.Pow(vel.y, 2));

        float damage = speed * (mass/10) * factor;

        this.TakeDamage(damage);

        IDamageTaker damageTaker;
        if (collision.collider.gameObject.TryGetComponent<IDamageTaker>(out damageTaker))
        {
            damageTaker.TakeDamage(damage);
        }
    }
}
