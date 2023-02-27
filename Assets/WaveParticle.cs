using UnityEngine;
using NaughtyAttributes;

public class WaveParticle : MonoBehaviour
{
    [SerializeField] private float dieTime = 2f;
    [SerializeField] private float damage = 0.5f;

    private GameObject particle;
    [SerializeField] private float baseMinDist = 0.1f;

    [Foldout("Info")] [DisableIf("true")][SerializeField] private float minDist = 0.1f;
    [Foldout("Info")][DisableIf("true")][SerializeField] private float angle;
    [Foldout("Info")][DisableIf("true")][SerializeField] private float speed;

    private WaveParticle right = null;
    private WaveParticle left = null;

    private void Start()
    {
        Destroy(this.gameObject, dieTime);
    }

    void Update()
    {
        dieTime -= Time.deltaTime;
        if (left == null && right == null)
        {
            Destroy(this.gameObject);
        }

        CheckRight();
    }

    public WaveParticle SetParticle(GameObject gameObject)
    {
        particle = gameObject;
        return this;
    }

    public WaveParticle SetAngle(float value)
    {
        angle = value;
        return this;
    }

    public WaveParticle SetSpeed(float value)
    {
        speed = value;
        return this;
    }

    public WaveParticle SetScale(float value)
    {
        transform.localScale = new Vector3(value, value, transform.localScale.z);
        minDist = baseMinDist * value;
        return this;
    }

    public WaveParticle SetDieTime(float value)
    {
        dieTime = value;
        return this;
    }

    public WaveParticle SetLeft(WaveParticle obj)
    {
        left = obj;
        if (obj != null)
        {
            obj.right = this;
        }
        return this;
    }

    void CheckRight()
    {
        if (right == null)
        {
            return;
        }

        float dist = Vector2.Distance(gameObject.transform.position, right.gameObject.transform.position);

        if (dist >= minDist)
        {
            float angleCalculated = (angle + right.angle) / 2f;

            float radian = angleCalculated * Mathf.PI / 180f;

            Vector2 pos = (gameObject.transform.position + right.gameObject.transform.position) / 2f;

            GameObject obj = Instantiate(particle, pos, new Quaternion(0, 0, 0, 0));

            right.SetLeft(obj.GetComponent<WaveParticle>());
            obj.GetComponent<WaveParticle>().SetParticle(particle).SetSpeed(speed).SetAngle(angleCalculated).SetDieTime(dieTime).SetScale(transform.localScale.x).SetLeft(this);

            obj.transform.eulerAngles = new Vector3(0, 0, angleCalculated);

            Rigidbody2D rb;
            if (obj.TryGetComponent<Rigidbody2D>(out rb))
            {
                rb.velocity = new Vector2(speed * Mathf.Cos(radian), speed * Mathf.Sin(radian));
            }
        }
    }

    public void LeftDied()
    {
        if (right == null)
        {
            if (this != null)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void RightDied()
    {
        if (left == null)
        {
            if (this != null)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageTaker damageTaker;
        if (collision.gameObject.TryGetComponent<IDamageTaker>(out damageTaker))
        {
            damageTaker.TakeDamage(damage);
        }

        WaveParticle wave;
        if (!collision.gameObject.TryGetComponent<WaveParticle>(out wave))
        {
            left?.RightDied();
            right?.LeftDied();
            Destroy(gameObject);
        }
    }
}
