using UnityEngine;

public class TestManager : MonoBehaviour
{
    public MonoBehaviour[] monoBehaviours;
    bool turnOn = true;

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
    }
}
