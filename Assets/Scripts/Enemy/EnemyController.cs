using UnityEngine;

public abstract class EnemyController : ObjectHealth
{
    [SerializeField] private SpriteRenderer bodySprite;
    [SerializeField] private ParticleSystem deathParticle;
    private Color damageColor = new(1, 79 / 255, 79 / 255);
    private Color normalColor = new(1, 1, 1);

    public Transform player;
    public float attackRecharge = 1f;
    public float range = 15f;

    [SerializeField] protected LayerMask viewRayBlockingLayers;

    private void OnValidate()
    {
        if (FindObjectOfType<PlayerControler>() != null)
            player = FindObjectOfType<PlayerControler>().transform;
    }

    public abstract void Attack();
    public override void TakeDamage(float value)
    {
        LeanTween.value(bodySprite.gameObject, SetSpriteColor, bodySprite.color, damageColor, 0.15f).setOnComplete(() => {
            LeanTween.value(bodySprite.gameObject, SetSpriteColor, bodySprite.color, normalColor, 0.15f);
        });
        base.TakeDamage(value);
    }
    public void SetSpriteColor(Color val)
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
