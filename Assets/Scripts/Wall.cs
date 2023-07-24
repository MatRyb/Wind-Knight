using UnityEngine;
using NaughtyAttributes;

public class Wall : MonoBehaviour
{
    private enum Type
    {
        Zero,
        Damage,
        Deadly
    }

    [SerializeField] private Type type = Type.Deadly;
    [SerializeField][ShowIf("ShowIfDamage")] private float cooldown;
    [SerializeField][ShowIf("ShowIfDamage")] private float damage;

    public bool ShowIfDamage => type == Type.Damage;
    private float time;

    private void Start()
    {
        if (type == Type.Damage)
        {
            time = cooldown;
        }
    }

    private void Update()
    {
        if (type == Type.Damage)
        {
            time -= Time.deltaTime * GameTimer.TimeMultiplier;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.TryGetComponent(out PlayerControler damageTaker) && time <= 0f)
        {
            switch (type)
            {
                case Type.Zero:
                    break;
                case Type.Deadly:
                    damageTaker.TakeDamage(damageTaker.GetMaxHealth());
                    break;
                case Type.Damage:
                    damageTaker.TakeDamage(damage);
                    time = cooldown;
                    break;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.gameObject.TryGetComponent(out PlayerControler damageTaker) && time <= 0f)
        {
            switch (type)
            {
                case Type.Zero:
                    break;
                case Type.Deadly:
                    damageTaker.TakeDamage(damageTaker.GetMaxHealth());
                    break;
                case Type.Damage:
                    damageTaker.TakeDamage(damage);
                    time = cooldown;
                    break;
            }
        }
    }
}
