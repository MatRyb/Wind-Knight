using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Pool;

public class WaveParticle : MonoBehaviour
{
    private float dieTime = 2f;
    [SerializeField] private float damage = 0.5f;

    [SerializeField] private float baseMinDist = 0.1f;

    [Foldout("Info")][DisableIf("true")][SerializeField] private float minDist = 0.1f;
    [Foldout("Info")][DisableIf("true")][SerializeField] private float angle;
    [Foldout("Info")][DisableIf("true")][SerializeField] private float speed;
    [Foldout("Info")][DisableIf("true")][SerializeField] private float radius;
    [Foldout("Info")] [DisableIf("true")] [SerializeField] private Color color;

    private WaveParticle right = null;
    private WaveParticle left = null;

    IObjectPool<WaveParticle> _pool;

    private LocalTimerContainer timer = null;

    private bool inPool = false;

    void Update()
    {
        if (_pool == null || inPool)
        {
            return;
        }

        dieTime -= Time.deltaTime * GameTimer.TimeMultiplier;

        if (GameTimer.TimeMultiplier == GameTimer.PLAYING)
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            float radian = angle * Mathf.PI / 180f;
            GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Mathf.Cos(radian), speed * Mathf.Sin(radian));

            if (left == null && right == null)
            {
                _pool.Release(this);
            }

            CheckRight();
        }
        else 
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
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

    public WaveParticle SetColor(Color value)
    {
        color = value;
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

    public WaveParticle SetInPool(bool value)
    {
        inPool = value;
        return this;
    }

    public WaveParticle SetLocalTimer(float time)
    {
        timer = LocalTimersManager.CreateNewTimer(time).DoAfter(() =>
        {
            _pool?.Release(this);
        }).Start();

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

    public WaveParticle SetRight(WaveParticle obj)
    {
        right = obj;
        if (obj != null)
        {
            obj.left = this;
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

            obj.GetComponent<WaveParticle>().SetSpeed(speed).SetAngle(angleCalculated).SetLocalTimer(dieTime).SetDieTime(dieTime).SetRadius(radius).SetColor(color).SetLeft(this);

            obj.transform.eulerAngles = new Vector3(0, 0, angleCalculated);

            obj.tag = gameObject.tag;

            if (obj.TryGetComponent(out SpriteRenderer s))
            {
                s.color = color;
            }

            if (obj.TryGetComponent(out Rigidbody2D rb))
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

        right?.SetLeft(null);
        left?.SetRight(null);
        left = null;
        right = null;

        if (TryGetComponent(out Rigidbody2D rb))
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageTaker damageTaker))
        {
            damageTaker.TakeDamage(damage);
        }

        if (!collision.gameObject.TryGetComponent(out WaveParticle _))
        {
            if (collision.gameObject.tag != "Checkpoint")
            {
                left?.RightDied();
                right?.LeftDied();

                if (_pool != null && !inPool)
                {
                    _pool.Release(this);
                }
            }
        }
    }
}
