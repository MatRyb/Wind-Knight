using UnityEngine;

public abstract class EnemyController : ObjectHealth
{
    public Transform player;
    public float attackRecharge = 1f;
    public float range = 15f;


    public abstract void Attack();

    public override void OnDead()
    {
        Destroy(gameObject);
    }
}
