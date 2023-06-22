using UnityEngine;

public class ExplosiveObjectScript : MonoBehaviour
{
    [SerializeField] private int time = 3;
    [SerializeField] private float explosionPower = 400f;
    [SerializeField] private float explosionDamage = 40f;
    [SerializeField] private float range = 15f;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private ParticleSystem destroyParticle;
    [SerializeField] private LayerMask nonActivatingLayers;
    [SerializeField] private string[] nonActivatingTags = { "Object" };

    [SerializeField] private Color deafultColor = new((87f / 255f), 0, 0, 100);
    [SerializeField] private Color blinkColor = new(100, 0, 0, 100);

    private float countdown = 0f;
    private float divider = 1f;
    private float counter = 0f;
    private bool start = false;

    private void OnValidate()
    {
        if (renderer == null && GetComponent<SpriteRenderer>() != null)
        {
            renderer = GetComponent<SpriteRenderer>();
        }
        else if (renderer == null && GetComponent<SpriteRenderer>() == null)
        {
            Debug.LogError(gameObject.name + " Sprite Renderer doesn't provided!!!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!start && !CheckTags(collision.gameObject) && !CheckLayers(collision.gameObject))
        {
            start = true;
            LeanTween.value(renderer.gameObject, SetSpriteColor, renderer.color, blinkColor, 0.15f).setOnComplete(() => {
                LeanTween.value(renderer.gameObject, SetSpriteColor, renderer.color, deafultColor, 0.15f);
            });
            CountingToExplode(1f / divider);
        }
    }

    private bool CheckTags(GameObject gameObject)
    {
        foreach (var tag in nonActivatingTags)
        {
            if (gameObject.CompareTag(tag))
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckLayers(GameObject gameObject)
    {
        int layerVal = 1 << gameObject.layer;

        if ((nonActivatingLayers & layerVal) >> gameObject.layer == 1)
        {
            return true;
        }

        return false;
    }

    private void CountingToExplode(float t)
    {
        LocalTimersManager.CreateNewTimer(t).DoAfter(() =>
        {
            LeanTween.value(renderer.gameObject, SetSpriteColor, renderer.color, blinkColor, 0.15f).setOnComplete(() => {
                LeanTween.value(renderer.gameObject, SetSpriteColor, renderer.color, deafultColor, 0.15f);
            });

            if (countdown > time)
            {
                Explode();
                OnDead();
            }
            else
            {
                counter += 1f;
                countdown += 1f / divider;

                if (counter >= countdown)
                {
                    counter = 0f;
                    divider += (float)(time - 1) / (float)time;
                }

                CountingToExplode(1f / divider);
            }

        }).Start();
    }

    public void SetSpriteColor(Color val)
    {
        renderer.color = val;
    }

    public void OnDead()
    {
        ParticleSystem particle = Instantiate(destroyParticle, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
        particle.Play();
        Destroy(particle.gameObject, 3);
        Destroy(gameObject);
    }

    private void Explode()
    {
        Collider2D[] collidersInExplosion = Physics2D.OverlapCircleAll(transform.position, range);
        if (collidersInExplosion.Length > 0)
        {
            foreach (var collider in collidersInExplosion)
            {
                if (collider.TryGetComponent<IDamageTaker>(out IDamageTaker damageTaker))
                {
                    if ((Object)damageTaker == this)
                    {
                        continue;
                    }

                    float powerPercente = 1f - (Mathf.Abs(Vector2.Distance(collider.gameObject.transform.position, transform.position)) / range);
                    float damage = Mathf.Clamp01(powerPercente) * explosionDamage;
                    damageTaker.TakeDamage(damage);
                }

                if (collider.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2D))
                {
                    Vector2 xy = rb2D.gameObject.transform.position - transform.position;

                    if (Vector2.Distance(rb2D.gameObject.transform.position, transform.position) == 0)
                    {
                        rb2D.AddForce(xy.normalized * (explosionPower), ForceMode2D.Impulse);
                    }
                    else
                    {
                        Vector2 d = (xy.normalized * (explosionPower)) / Vector2.Distance(rb2D.gameObject.transform.position, transform.position);
                        rb2D.AddForce(new Vector2((d.x / 2.5f), (d.y * 2)), ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, range);
    }
}
