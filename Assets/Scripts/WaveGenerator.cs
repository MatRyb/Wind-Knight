using UnityEngine;
using NaughtyAttributes;
using System.Collections;

public class WaveGenerator : MonoBehaviour
{
    [SerializeField] private uint amount = 3_0_0;

    // change to radius of particle
    [SerializeField] private uint thickness = 2;

    [SerializeField] private float offset = 1f;

    [SerializeField] public GameObject particle;

    [SerializeField] private float speed = 10f;

    [SerializeField] [Range(0, 360)] private float angle = 45f;

    [Button]
    public void spawnWave()
    {
        float stepPerLayer = angle / (amount - 1);

        WaveParticle left = null;
        for (uint z = 0; z < amount; ++z)
        {
            float angleCalculated = -(angle / 2f) + stepPerLayer * z + gameObject.transform.eulerAngles.z;

            float radian = angleCalculated * Mathf.PI / 180f;

            float lookRadian = gameObject.transform.eulerAngles.z * Mathf.PI / 180f;

            GameObject obj = Instantiate(particle, gameObject.transform.position + new Vector3(offset * Mathf.Cos(lookRadian), offset * Mathf.Sin(lookRadian)), new Quaternion(0, 0, 0, 0));

            left = obj.GetComponent<WaveParticle>().SetParticle(particle).SetSpeed(speed).SetAngle(angleCalculated).SetScale(thickness).SetLeft(left);

            obj.transform.eulerAngles = new Vector3(0, 0, angleCalculated);

            Rigidbody2D rb;
            if (obj.TryGetComponent<Rigidbody2D>(out rb))
            {
                rb.velocity = new Vector2(speed * Mathf.Cos(radian), speed * Mathf.Sin(radian));
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        float lookRadian = gameObject.transform.eulerAngles.z * Mathf.PI / 180f;
        Gizmos.DrawWireSphere(gameObject.transform.position + new Vector3(offset * Mathf.Cos(lookRadian), offset * Mathf.Sin(lookRadian)), 0.5f);
    }
}
