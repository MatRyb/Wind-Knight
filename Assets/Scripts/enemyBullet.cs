using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBullet : MonoBehaviour
{
    public float dieTime;
    public float damage;

    void Start()
    {
         

        Destroy(gameObject, dieTime);
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            PlayerControler playerC = collider.gameObject.GetComponent(typeof(PlayerControler)) as PlayerControler;
            playerC.hitPoints -= damage;
            if (playerC.hitPoints <= 0.0f)
            {
                Debug.Log("You ded");
            }
        }
        Destroy(gameObject);
    }

    /* void Destroyer()
    {
        //"I am become Death, the destroyer of worlds"
        Destroy(gameObject, dieTime);
    }*/
}
