using UnityEngine;

public class VertexDecoder : MonoBehaviour
{
    static GameObject DecodeLoop(Texture2D map)
    {
        if (map == null)
        {
            Debug.LogError("Vertex Decoder: Passed Texture in decode loop is null.");
            return null;
        }

        GameObject handler = new("MeshVertex");
        handler.transform.position = Vector3.zero;
        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                Color pixel = map.GetPixel(x, y);
                if (pixel.a != 0f && DecodeID(pixel) == 0)
                {
                    DecodeVertexes(x, y, handler, map);
                }
            }
        }

        return handler;
    }

    static void DecodeVertexes(int x, int y, GameObject handler, Texture2D map)
    {
        int currX = x, currY = y;
        for (int i = 0; i < 4; i++)
        {
            GameObject vertex = new("Vertex (" + i + ", " + currX + ", " + currY + ")");
            vertex.transform.position = new Vector3(currX, currY);
            vertex.transform.parent = handler.transform;
            (currX, currY) = DecodePos(map.GetPixel(currX, currY));
        }
    }

    static int DecodeID(Color color)
    {
        return ((int)(color.r * 255f) & 0xC0) >> 6;
    }

    static (int, int) DecodePos(Color color)
    {
        //int id = 0;
        int x = 0;
        int y = 0;

        int r = (int)(color.r * 255f);
        int g = (int)(color.g * 255f);
        int b = (int)(color.b * 255f);

        //id = (r & 0xC0) >> 6;
        x = ((r & 0x3F) << 5) | ((g & 0xF8) >> 3);
        y = ((g & 0x07) << 8) | b;

        return (x, y);
    }

    public static GameObject DecodeVertexMap(Texture2D tex)
    {
        if (tex == null)
        {
            Debug.LogError("Vertex Decoder: Passed Texture is null.");
            return null;
        }

        return DecodeLoop(tex);
    }
}

