using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBullet : MonoBehaviour
{
    public float dieTime;
    public float damage;
    public Transform player;


    void Start()
    {
        Destroy(gameObject, dieTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    /* void Destroyer()
    {
        //"I am become Death, the destroyer of worlds"
        Destroy(gameObject, dieTime);
    }*/
}
