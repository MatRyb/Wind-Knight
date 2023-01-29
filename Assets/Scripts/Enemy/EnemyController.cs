using UnityEngine;
using UnityEngine.UI;

public abstract class EnemyController : ObjectHealth
{
    [SerializeField] private SpriteRenderer bodySprite;
    [SerializeField] private ParticleSystem deathParticle;
    private Color damageColor = new Color(1, 79 / 255, 79 / 255);
    private Color normalColor = new Color(1, 1, 1);

    public Transform player;
    public float attackRecharge = 1f;
    public float range = 15f;

    private void OnValidate()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public abstract void Attack();
    public override void TakeDamage(float value)
    {
        LeanTween.value(bodySprite.gameObject, setSpriteColor, bodySprite.color, damageColor, 0.15f).setOnComplete(() => {
            LeanTween.value(bodySprite.gameObject, setSpriteColor, bodySprite.color, normalColor, 0.15f);
        });
        base.TakeDamage(value);
    }
    public void setSpriteColor(Color val)
    {
        bodySprite.color = val;
    }

    public override void OnDead()
    {
        ParticleSystem particle = Instantiate(deathParticle, this.gameObject.transform.position, new Quaternion(0, 0, 0, 0));
        particle.Play();
        Destroy(particle, 3);
        Destroy(gameObject);
    }
}
