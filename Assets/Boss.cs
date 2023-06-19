using UnityEngine;

public class Boss : MonoBehaviour
{
    int i = 0;
    Vector2 pos = new(0.01f, -0.01f);

    void Update()
    {
        transform.Translate(pos);

        i++;

        if(i%1000 == 0)
        {
            pos *= new Vector2(-1, 1);
        }


    }
}
