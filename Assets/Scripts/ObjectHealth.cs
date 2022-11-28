using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour, IDamageTaker
{   
    [Header("Health:")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health = 0f;

    void Awake()
    {
        health = maxHealth;
    }

    public void TakeDamage(float value)
    {
        health -= value;
        if(health <= 0f)
        {
            health = 0f;
            OnDead();
        }
    }

    public virtual void OnDead()
    {
        Debug.Log("Dead");
    }
}
