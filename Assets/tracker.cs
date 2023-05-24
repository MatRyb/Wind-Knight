using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tracker : MonoBehaviour
{
    public Transform tracked;

    // Update is called once per frame
    void Update()
    {
        transform.position = tracked.position;
    }
}
