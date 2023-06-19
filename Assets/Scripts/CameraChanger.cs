using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CameraChanger : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;

    private float size = 14f;

    private bool outside = true;

    [Foldout("Info:")]
    [DisableIf("true")][SerializeField] private List<BoxCollider2D> staticColliders = new List<BoxCollider2D>();

    private void Awake()
    {
        foreach (GameObject item in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if(item.layer == LayerMask.NameToLayer("CameraViewCollider"))
            {
                staticColliders.Add(item.GetComponent<BoxCollider2D>());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
            LeanTween.cancel(cam.gameObject);
            outside = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
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

        if (isOneOfUs && !outside)
        {
            //Debug.Log("stay");
            outside = true;
            BoxCollider2D box = (BoxCollider2D)collision;
            cam.GetComponent<CameraFolow>().enabled = false;
            LeanTween.value(cam.gameObject, ChangeCamTransform, cam.transform.position, new Vector3(collision.transform.position.x, collision.transform.position.y, cam.transform.position.z), 0.25f);
            if (box.size.y > box.size.x)
            {
                //cam.orthographicSize = .54f * box.size.y - 3.73f;
                LeanTween.value(cam.gameObject, ChangeCamOrthographicSize, cam.orthographicSize, .32f * box.size.y - 3.73f, 0.25f);
            }
            else
            {
                //cam.orthographicSize = .54f * box.size.x - 3.73f;
                LeanTween.value(cam.gameObject, ChangeCamOrthographicSize, cam.orthographicSize, .32f * box.size.x - 3.73f, 0.25f);
            }
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

        bool isStillIn = true;

        Collider2D trigger = Physics2D.OverlapCircle(target.position, 0.1f);

        foreach (BoxCollider2D col in staticColliders)
        {
            isStillIn = (col == trigger && col.Equals(trigger));

            if (isStillIn)
            {
                break;
            }
        }

        if (isOneOfUs && outside && !isStillIn)
        {
            if (target.gameObject.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                LeanTween.value(cam.gameObject, ChangeCamTransform, cam.transform.position, new Vector3(target.position.x + rb.velocity.x * 0.5f, target.position.y + rb.velocity.y * 0.5f, cam.transform.position.z), 0.25f)
                    .setOnComplete(() => {
                        cam.GetComponent<CameraFolow>().enabled = true;
                    });
                LeanTween.value(cam.gameObject, ChangeCamOrthographicSize, cam.orthographicSize, size, 0.25f);
            }
            else
            {
                LeanTween.value(cam.gameObject, ChangeCamTransform, cam.transform.position, new Vector3(target.position.x, target.position.y, cam.transform.position.z), 0.25f)
                    .setOnComplete(() => {
                        cam.GetComponent<CameraFolow>().enabled = true;
                    });
                LeanTween.value(cam.gameObject, ChangeCamOrthographicSize, cam.orthographicSize, size, 0.25f);
            }
        }
        else if (isOneOfUs && outside && isStillIn)
        {
            outside = false;
        }
    }

    public void ChangeCamTransform(Vector3 val)
    {
        cam.transform.position = val;
    }

    public void ChangeCamOrthographicSize(float val)
    {
        cam.orthographicSize = val;
    }
}
