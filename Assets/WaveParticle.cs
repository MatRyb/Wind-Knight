using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Pool;

public class WaveParticle : MonoBehaviour
{
    [SerializeField] private float dieTime = 2f;
    [SerializeField] private float damage = 0.5f;

    [SerializeField] private float baseMinDist = 0.1f;

    [Foldout("Info")][DisableIf("true")][SerializeField] private float minDist = 0.1f;
    [Foldout("Info")][DisableIf("true")][SerializeField] private float angle;
    [Foldout("Info")][DisableIf("true")][SerializeField] private float speed;
    [Foldout("Info")][DisableIf("true")][SerializeField] private float radius;

    private WaveParticle right = null;
    private WaveParticle left = null;

    IObjectPool<WaveParticle> _pool;

    private LocalTimerContainer timer = null;

    void Update()
    {
        if (_pool == null)
        {
            return;
        }

        dieTime -= Time.deltaTime;

        if (left == null && right == null)
        {
            ResetParameters();
            _pool.Release(this);
        }

        CheckRight();
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

    public WaveParticle SetRadius(float value)
    {
        const float ratio = 1f / 0.5f;
        radius = value;
        transform.localScale = new Vector3(value * ratio, value * ratio, transform.localScale.z);
        minDist = baseMinDist * value;
        return this;
    }

    public WaveParticle SetDieTime(float value)
    {
        dieTime = value;
        return this;
    }

    public WaveParticle SetLocalTimer(LocalTimerContainer value)
    {
        timer = value;
        return this;
    }

    public float GetDieTime()
    {
        return dieTime;
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

    public void SetPool(IObjectPool<WaveParticle> pool) => _pool = pool;

    void CheckRight()
    {
        if (right == null || _pool == null)
        {
            return;
        }

        float dist = Vector2.Distance(gameObject.transform.position, right.gameObject.transform.position);

        if (dist >= minDist)
        {
            float angleCalculated = (angle + right.angle) / 2f;

            float radian = angleCalculated * Mathf.PI / 180f;

            Vector2 pos = (gameObject.transform.position + right.gameObject.transform.position) / 2f;

            WaveParticle obj = _pool.Get();

            obj.transform.SetPositionAndRotation(pos, new Quaternion(0, 0, 0, 0));

            right.SetLeft(obj.GetComponent<WaveParticle>());

            obj.GetComponent<WaveParticle>().SetSpeed(speed).SetAngle(angleCalculated).SetDieTime(dieTime).SetRadius(radius).SetLeft(this);

            obj.transform.eulerAngles = new Vector3(0, 0, angleCalculated);

            if (obj.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
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
                if (_pool != null)
                {
                    _pool.Release(this);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void RightDied()
    {
        if (left == null)
        {
            if (_pool != null)
            {
                _pool.Release(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void ResetParameters()
    {
        timer?.Stop();

        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageTaker damageTaker;
        if (collision.gameObject.TryGetComponent<IDamageTaker>(out damageTaker))
        {
            damageTaker.TakeDamage(damage);
        }

        if (!collision.gameObject.TryGetComponent<WaveParticle>(out WaveParticle _))
        {
            left?.RightDied();
            right?.LeftDied();

            if (_pool != null)
            {
                _pool.Release(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
