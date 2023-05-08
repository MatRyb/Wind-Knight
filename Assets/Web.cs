using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Web : MonoBehaviour
{
    public Rigidbody2D hook;
    public GameObject[] prefabStringSegments;
    public GameObject hatch;
    public int numLinks = 5;
    

    void Start()
    {
        StartCoroutine(waiter());
        //GenerateWeb();
    }

    void GenerateWeb()
    {
        Rigidbody2D prevBody = hook;
        for (int i = 0; i < numLinks; i++)
        {
            StartCoroutine(waiter());
            int index = UnityEngine.Random.Range(0, prefabStringSegments.Length);
            GameObject newSeg;
            if (i == numLinks - 1)
            {
                newSeg = Instantiate(hatch);
            }
            else
            {
                newSeg = Instantiate(prefabStringSegments[index]);
            }
            newSeg.name = "seg" + i;
            newSeg.transform.parent = transform;
            newSeg.transform.position = transform.position;
            HingeJoint2D hj = newSeg.GetComponent<HingeJoint2D>();
            hj.connectedBody = prevBody;

            prevBody = newSeg.GetComponent<Rigidbody2D>();
        }

    }
    private IEnumerator waiter()
    {
        Rigidbody2D prevBody = hook;
        for (int i = 0; i < numLinks; i++)
        {
            yield return new WaitForSeconds(0.01f);

            int index = UnityEngine.Random.Range(0, prefabStringSegments.Length);
            GameObject newSeg;
            if (i == numLinks - 1)
            {
                newSeg = Instantiate(hatch);
            }
            else
            {
                newSeg = Instantiate(prefabStringSegments[index]);
            }
            newSeg.name = "seg" + i;
            newSeg.transform.parent = transform;
            newSeg.transform.position = transform.position;
            HingeJoint2D hj = newSeg.GetComponent<HingeJoint2D>();
            hj.connectedBody = prevBody;

            prevBody = newSeg.GetComponent<Rigidbody2D>();
        }
    }
}
