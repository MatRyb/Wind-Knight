using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hatchScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 7)
        {
            gameObject.GetComponent<FixedJoint2D>().enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
