using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    public float parallaxFactor;
    private float length, startPosX, startPosY;
    public GameObject cam;

    void Start()
    {
        if (cam == null)
        {
            cam = GameObject.FindWithTag("MainCamera");
        }

        startPosX = transform.position.x;
        startPosY = transform.position.y;

        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        if (cam == null)
        {
            cam = GameObject.FindWithTag("MainCamera");
            if (cam == null)
            {
                Debug.LogWarning("No camera found with tag 'MainCamera'");
                return;
            }
        }

        float temp = (cam.transform.position.x * (1 - parallaxFactor));
        float dist = (cam.transform.position.x * parallaxFactor);
        float distY = (cam.transform.position.y);

        transform.position = new Vector3(startPosX + dist, startPosY + distY, transform.position.z);

        if (temp > startPosX + length) startPosX += length;
        else if (temp < startPosX - length) startPosX -= length;
    }


}