using UnityEngine;
using NaughtyAttributes;

public class ObjectScript : MonoBehaviour, IDamageTaker
{
    [Header("Health:")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health = 0f;

    [Header("Rigidbody:")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float mass = 0f;
    [SerializeField] private float linearDrag = 0f;

    [Header("Damage:")]
    [SerializeField] private float factor = .2f;

    [Foldout("Info")]
    [DisableIf("true")] [SerializeField] private Vector2 velocity;

    void Start()
    {
        if(this.gameObject.GetComponent<Rigidbody2D>() == null)
        {
            this.gameObject.AddComponent<Rigidbody2D>();
        }

        if (this.gameObject.GetComponent<Collider2D>() == null)
        {
            Debug.LogError("ObjectScript -> NO COLLIDER ATTACHED");
        }

        rb = this.GetComponent<Rigidbody2D>();
        rb.mass = mass;
        rb.drag = linearDrag;

        health = maxHealth;
    }

    void Update()
    {
        if (rb.velocity.x != 0.0f || rb.velocity.y != 0.0f)
        {
            velocity = rb.velocity;
        }
    }

    public void TakeDamage(float value)
    {
        health -= value;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 vel = ComputeIncidentVelocity(collision);

        float speed = Mathf.Sqrt(Mathf.Pow(vel.x, 2) + Mathf.Pow(vel.y, 2));

        float damage = speed * (mass/10) * factor;

        health -= damage;

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
