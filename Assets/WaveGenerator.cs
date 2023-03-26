using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Pool;

public class WaveGenerator : MonoBehaviour
{
    [SerializeField] private uint amount = 3;

    // change to radius of particle
    [SerializeField] private float thickness = 0.5f;

    [SerializeField] private float offset = 3f;

    [SerializeField] public WaveParticle prefab;

    [SerializeField] private float speed = 10f;

    [SerializeField] [Range(0, 180)] private float angle = 45f;

    private ObjectPool<WaveParticle> _pool;

    [SerializeField] private int inactiveCount;

    [SerializeField] private int activeCount;

    void Awake() => _pool = new ObjectPool<WaveParticle>(CreateParticle, OnTakeParticleFromPool, OnReturnParticleToPool);

    private void Update()
    {
        inactiveCount = _pool.CountInactive;
        activeCount = _pool.CountActive;
    }

    [Button]
    public void SpawnWave()
    {
        float stepPerLayer = angle / (amount - 1);

        WaveParticle left = null;
        for (uint z = 0; z < amount; ++z)
        {
            float angleCalculated = -(angle / 2f) + stepPerLayer * z + gameObject.transform.eulerAngles.z;

            float radian = angleCalculated * Mathf.PI / 180f;

            float lookRadian = gameObject.transform.eulerAngles.z * Mathf.PI / 180f;

            WaveParticle obj = _pool.Get();

            obj.transform.SetPositionAndRotation(gameObject.transform.position + new Vector3(offset * Mathf.Cos(lookRadian), offset * Mathf.Sin(lookRadian)), new Quaternion(0, 0, 0, 0));

            left = obj.GetComponent<WaveParticle>().SetSpeed(speed).SetAngle(angleCalculated).SetRadius(thickness).SetLeft(left);

            if (obj.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                rb.velocity = new Vector2(speed * Mathf.Cos(radian), speed * Mathf.Sin(radian));
            }
        }
    }

    WaveParticle CreateParticle()
    {
        WaveParticle particle = Instantiate(prefab);
        particle.SetPool(_pool);
        particle.SetInPool(false);

        particle.SetLocalTimer(LocalTimersManager.CreateNewTimer(particle.GetDieTime()).DoAfter(() =>
        {
            _pool.Release(particle);
        }));

        return particle;
    }

    void OnTakeParticleFromPool(WaveParticle particle)
    {
        particle.gameObject.SetActive(true);
        particle.SetInPool(false);

        particle.SetLocalTimer(LocalTimersManager.CreateNewTimer(particle.GetDieTime()).DoAfter(() =>
        {
            _pool.Release(particle);
        }));
    }

    void OnReturnParticleToPool(WaveParticle particle)
    {
        particle.gameObject.SetActive(false);
        particle.SetInPool(true);
        particle.ResetParameters();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        float lookRadian = gameObject.transform.eulerAngles.z * Mathf.PI / 180f;
        Gizmos.DrawWireSphere(gameObject.transform.position + new Vector3(offset * Mathf.Cos(lookRadian), offset * Mathf.Sin(lookRadian)), 0.5f);
    }
}
