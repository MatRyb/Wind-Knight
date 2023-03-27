using UnityEngine;
using UnityEngine.Pool;

public class WavePoolManager : MonoBehaviour
{
    private ObjectPool<WaveParticle> _pool;

    [SerializeField] public WaveParticle prefab;

    [SerializeField] private int inactiveCount;

    [SerializeField] private int activeCount;

    public static WavePoolManager instance;

    void Awake()
    {
        _pool = new ObjectPool<WaveParticle>(CreateParticle, OnTakeParticleFromPool, OnReturnParticleToPool);
        instance = this;
    }

    private void Update()
    {
        inactiveCount = _pool.CountInactive;
        activeCount = _pool.CountActive;
    }

    public ObjectPool<WaveParticle> GetPool()
    {
        return _pool;
    }

    WaveParticle CreateParticle()
    {
        WaveParticle particle = Instantiate(prefab);
        particle.SetPool(_pool);
        particle.SetInPool(false);

        return particle;
    }

    void OnTakeParticleFromPool(WaveParticle particle)
    {
        particle.gameObject.SetActive(true);
        particle.SetInPool(false);
    }

    void OnReturnParticleToPool(WaveParticle particle)
    {
        particle.gameObject.SetActive(false);
        particle.SetInPool(true);
        particle.ResetParameters();
    }
}
