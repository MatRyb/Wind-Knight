using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private BoxCollider2D[] staticColliders;
    [SerializeField] private Transform target;

    private float size = 14f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isOneOfUs = false;

        foreach(BoxCollider2D col in staticColliders)
        {
            isOneOfUs = (col == collision && col.Equals(collision));

            if (isOneOfUs)
            {
                break;
            }
        }

        if(isOneOfUs)
        {
            BoxCollider2D box = (BoxCollider2D)collision;
            cam.GetComponent<CameraFolow>().enabled = false;
            cam.transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y, cam.transform.position.z);
            cam.orthographicSize = .54f * box.size.y - 3.73f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        bool isOneOfUs = false;

        foreach (BoxCollider2D col in staticColliders)
        {
            isOneOfUs = (col == collision && col.Equals(collision));

            if (isOneOfUs)
            {
                break;
            }
        }

        if (isOneOfUs)
        {
            cam.orthographicSize = size;
            cam.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, cam.transform.position.z);
            cam.GetComponent<CameraFolow>().enabled = true;
        }
    }
}
