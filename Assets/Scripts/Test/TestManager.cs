using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public MonoBehaviour[] monoBehaviours;
    bool turnOn = true;

    public List<EnemyController> enemys = new List<EnemyController>();

    void Awake()
    {
        foreach (var obj in monoBehaviours)
        {
            if (obj.enabled)
            {
                obj.enabled = false;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var obj in monoBehaviours)
            {
                if (turnOn)
                    obj.enabled = true;
                else
                    obj.enabled = false;
            }

            turnOn = !turnOn;
        }

        enemys.Clear();
        enemys.AddRange(GameObject.FindObjectsOfType<EnemyController>());
        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (var obj in enemys)
            {
                obj.TakeDamage(1000f);
            }
        }
    }
}
