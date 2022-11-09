using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public MonoBehaviour[] monoBehaviours;
    bool turnOn = true;

    // Start is called before the first frame update
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

    // Update is called once per frame
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
    }
}
