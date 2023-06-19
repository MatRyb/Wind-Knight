using System.Collections;
using UnityEngine;

public class StringScript : MonoBehaviour
{
    public GameObject connectedAbove, connectedBelow;

    void Start()
    {
        connectedAbove = GetComponent<HingeJoint2D>().connectedBody.gameObject;
        StringScript aboveSegment = connectedAbove.GetComponent<StringScript>();
        if(aboveSegment != null)
        {
            aboveSegment.connectedBelow = gameObject;
            float spriteBottom = connectedAbove.GetComponent<SpriteRenderer>().bounds.size.y;
            //GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, spriteBottom * -7);
            GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0,-5);
        }
        else
        {
            GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, 0);
        }
        StartCoroutine(waiter());
    }

    private IEnumerator waiter()
    {
        yield return new WaitForSeconds(3);
        gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
    }
}
