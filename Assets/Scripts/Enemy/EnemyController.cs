using UnityEngine;
using NaughtyAttributes;

public abstract class EnemyController : ObjectHealth
{
    [HideInInspector] public bool attacking = false;
    [SerializeField] private bool isPaperScrap = false;
    [SerializeField][ShowIf("isPaperScrap")] private GameObject paperScrap;
    private int paperScrapId = 0;
    [SerializeField] private SpriteRenderer bodySprite;
    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private GameObject objectToDestroy;
    private Color damageColor = new(1, 79 / 255, 79 / 255);
    private Color normalColor = new(1, 1, 1);

    public Transform player;
    public float attackRecharge = 1f;
    public float range = 15f;

    [SerializeField] protected LayerMask viewRayBlockingLayers;

    [SerializeField] protected GameObject source;
    [SerializeField] protected AudioClip attackClip;
    [SerializeField] private AudioClip dieClip;

    protected float volume = 0f;
    protected bool mute = false;

    private void Awake()
    {
        if (FindObjectOfType<PlayerControler>() != null)
            player = FindObjectOfType<PlayerControler>().transform;
    }

    private void Start()
    {
        volume = OptionsLevelManager.instance.GetSFXVolume();
        mute = OptionsLevelManager.instance.GetSFXMute();
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

    public void SetIsPaperScrap(bool value)
    {
        isPaperScrap = value;
    }

    public void SetPaperScrapId(int value)
    {
        paperScrapId = value;
    }

    public bool HavePaperScrap()
    {
        return isPaperScrap;
    }

    public override void OnDead()
    {
        ParticleSystem particle = Instantiate(deathParticle, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
        particle.Play();
        Destroy(particle.gameObject, 3);

        if (isPaperScrap)
        {
            GameObject scrap = Instantiate(paperScrap, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
            scrap.GetComponent<PaperScrap>().SetId(paperScrapId);
        }

        AudioSource s = Instantiate(source, transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<AudioSource>();
        s.clip = dieClip;
        s.volume = volume;
        s.mute = mute;
        s.Play();
        Destroy(s.gameObject, 2f);
        Destroy(objectToDestroy != null ? objectToDestroy : gameObject);
    }
}
