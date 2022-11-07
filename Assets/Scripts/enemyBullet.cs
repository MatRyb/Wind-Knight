using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBullet : MonoBehaviour
{
    public float dieTime;
    public float damage;
    public Transform player;
    public float bulletSpeed;
    void Start()
    {
        Destroyer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    void Destroyer()
    {
        //"I am become Death, the destroyer of worlds"
        Destroy(gameObject, dieTime);
    }
}
