using UnityEngine;

public abstract class EnemyController : ObjectHealth
{
    public Transform player;
    public float attackRecharge;
    public float range;


    public abstract void Attack();

    public override void OnDead()
    {
        Destroy(gameObject);
    }
}
