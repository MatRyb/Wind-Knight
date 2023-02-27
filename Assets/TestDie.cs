using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDie : MonoBehaviour
{
    [SerializeField] private float dieTime = 1f;

    private void Start()
    {
        Destroy(this.gameObject, dieTime);
    }
}
