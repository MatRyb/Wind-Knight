using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    public float dieTime;
    public float damage;

    private LocalTimer dieTimer;

    void Start()
    {

        Destroy(gameObject, dieTime);
        /* z jakiegos powodu nie chce dzialac :c
          dieTimer = new LocalTimer(dieTime).DoAfter(() => Destroy(this.gameObject)).Start();
        */
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Contact!");
        if (collision.gameObject.tag == "Player")
        {
            IDamageTaker damageTaker;
            if (collision.transform.gameObject.TryGetComponent<IDamageTaker>(out damageTaker))
            {
                damageTaker.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
