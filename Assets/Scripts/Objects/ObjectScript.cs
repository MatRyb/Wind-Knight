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
        Debug.Log("Object Dead");
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

    static Vector2 ComputeTotalImpulse(Collision2D collision)
    {
        Vector2 impulse = Vector2.zero;
        int contactCount = collision.contactCount;
        for (int i = 0; i < contactCount; i++)
        {
            var contact = collision.GetContact(i);
            impulse += contact.normal * contact.normalImpulse;
            impulse.x += contact.tangentImpulse * contact.normal.y;
            impulse.y -= contact.tangentImpulse * contact.normal.x;
        }
        return impulse;
    }
    static Vector2 ComputeIncidentVelocity(Collision2D collision)
    {
        Vector2 impulse = ComputeTotalImpulse(collision);
        var myBody = collision.otherRigidbody;
        return myBody.velocity - impulse / myBody.mass;
    }
}
