using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web : MonoBehaviour
{
    public Rigidbody2D hook;
    public GameObject[] prefabStringSegments;
    public int numLinks = 5;
    

    void Start()
    {
        GenerateWeb();
    }

    void GenerateWeb()
    {
        Rigidbody2D prevBody = hook;
        for(int i = 0; i < numLinks; i++)
        {
            
            int index = UnityEngine.Random.Range(0, prefabStringSegments.Length);
            GameObject newSeg = Instantiate(prefabStringSegments[index]);
            newSeg.transform.parent = transform;
            newSeg.transform.position = transform.position;
            HingeJoint2D hj = newSeg.GetComponent<HingeJoint2D>();
            hj.connectedBody = prevBody;

            prevBody = newSeg.GetComponent<Rigidbody2D>();
        }
    }
}
