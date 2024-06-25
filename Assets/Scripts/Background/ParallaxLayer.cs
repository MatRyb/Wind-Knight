using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    public float parallaxFactor;
    private float length, startPosX, startPosY;
    public GameObject cam;

    void Start()
    {
        startPosX = transform.position.x;
        startPosY = transform.position.y;

        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float temp = (cam.transform.position.x * (1 - parallaxFactor));
        float dist = (cam.transform.position.x * parallaxFactor);
        float distY = (cam.transform.position.y);

        transform.position = new Vector3(startPosX + dist, startPosY + distY, transform.position.z);

        if (temp > startPosX + length) startPosX += length;
        else if (temp < startPosX - length) startPosX -= length;
    }


}