using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    public double enemyHealth = 0f;
    public Transform player;
    public float attackRecharge;
    public float range;
    public virtual void Attack ()
    {

    }


}
