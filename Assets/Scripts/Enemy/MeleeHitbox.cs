using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    public float dieTime;
    public float damage;

    void Start()
    {
        //Destroy(gameObject, dieTime);
        LocalTimersManager.CreateNewTimer(dieTime).DoAfter(() => Destroy(gameObject)).Start();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerControler>(out PlayerControler _))
        {
            if (collision.transform.gameObject.TryGetComponent<IDamageTaker>(out IDamageTaker damageTaker))
            {
                damageTaker.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
