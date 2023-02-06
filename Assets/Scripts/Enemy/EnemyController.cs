using UnityEngine;
using System.Collections.Generic;

public abstract class EnemyController : ObjectHealth
{
    [SerializeField] private SpriteRenderer bodySprite;
    [SerializeField] private ParticleSystem deathParticle;
    private Color damageColor = new Color(1, 79 / 255, 79 / 255);
    private Color normalColor = new Color(1, 1, 1);

    public Transform player;
    public float attackRecharge = 1f;
    public float range = 15f;

    [SerializeField] private LayerMask viewRayBlockingLayers;

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

    public bool isObjectBlockedByOtherObject(GameObject objectToCheck)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, (objectToCheck.transform.position - transform.position).normalized, Mathf.Abs(Vector3.Distance(objectToCheck.transform.position, transform.position)), viewRayBlockingLayers);
        if (hits.Length == 0)
        {
            return true;
        }

        List<RaycastHit2D> allowedHits = new List<RaycastHit2D>();
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject != this.gameObject)
                allowedHits.Add(hit);
        }
        if (allowedHits.Count == 0)
        {
            return true;
        }

        RaycastHit2D closestHit = allowedHits[0];
        float minDist = Mathf.Abs(Vector3.Distance(closestHit.point, transform.position));
        for (int i = 1; i < allowedHits.Count; i++)
        {
            float dist = Mathf.Abs(Vector3.Distance(allowedHits[i].point, transform.position));
            if (dist < minDist)
            {
                minDist = dist;
                closestHit = allowedHits[i];
            }
        }
        return closestHit.collider.gameObject != objectToCheck;
    }
}
