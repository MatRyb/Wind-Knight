using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float dieTime;
    public float damage;

    private LocalTimer dieTimer;
    private Vector2 velocity;

    void Start()
    {
        //"Now I am become Death, the destroyer of worlds"
        dieTimer = new LocalTimer(dieTime).DoAfter(() => Destroy(gameObject)).Start();
        velocity = GetComponent<Rigidbody2D>().velocity;
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().velocity = velocity * GameTimer.TimeMultiplier;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageTaker damageTaker;
        if (collision.collider.gameObject.TryGetComponent<IDamageTaker>(out damageTaker))
        {
            damageTaker.TakeDamage(damage);
        }

        /*
        if (collision.gameObject.tag == "Player")
        {
            PlayerControler playerC = collision.gameObject.GetComponent(typeof(PlayerControler)) as PlayerControler;
            playerC.health -= damage;
            if (playerC.health <= 0.0f)
            {
                Debug.Log("You ded");
            }
        }
        */

        Destroy(gameObject);
    }
}