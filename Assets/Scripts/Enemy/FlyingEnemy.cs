using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : EnemyController
{
    [SerializeField] private Rigidbody2D enemyRigidbody;
    [SerializeField] private float speed = 10f;

    [SerializeField] private bool attack = false;
    [SerializeField] private bool settingOff = false;
    [SerializeField] private float detonationRange = 3f;
    [SerializeField] private float setOffTime = 2f;

    private void FixedUpdate()
    {
        if (attack && !settingOff)
        {
            Vector3 dir = player.position - this.gameObject.transform.position;

            if (Mathf.Abs(Vector3.Distance(Vector3.zero, dir)) > detonationRange)
            {
                Vector3 vel = dir.normalized * speed;
                enemyRigidbody.velocity = vel;
            }
            else
            {
                enemyRigidbody.velocity = Vector3.zero;
                Attack();
            }
        }   
    }

    public override void Attack()
    {
        Debug.Log("SETTING OFF");
        settingOff = true;
        LocalTimersManager.CreateNewTimer(setOffTime).DoAfter(() =>
        {
            Debug.Log("EXPLOSION");
        }).Start();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !attack)
        {
            player = collision.gameObject.transform;
            attack = true;
        }
    }
}
