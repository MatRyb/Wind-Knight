using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    [SerializeField] private float dieTime;
    [SerializeField] private float damage;
    private LocalTimerContainer timer;

    void Start()
    {
        //Destroy(gameObject, dieTime);
        timer = LocalTimersManager.CreateNewTimer(dieTime).DoAfter(() => Destroy(gameObject)).Start();
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

    private void OnDestroy()
    {
        timer.Stop();
    }
}
