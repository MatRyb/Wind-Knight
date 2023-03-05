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
    [SerializeField][ShowIf("ShowIfDamage")] private float damage;

    public bool ShowIfDamage => type == Type.Damage;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerControler damageTaker;
        if (collision.collider.gameObject.TryGetComponent<PlayerControler>(out damageTaker))
        {
            switch (type)
            {
                case Type.Zero:
                    break;
                case Type.Deadly:
                    damageTaker.TakeDamage(damageTaker.getMaxHealth());
                    break;
                case Type.Damage:
                    damageTaker.TakeDamage(damage);
                    break;
            }
        }
    }
}
