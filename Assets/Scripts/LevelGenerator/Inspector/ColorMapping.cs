using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ColorMapping
{
    public Color color = Color.black;
    public MappingType type;
    public GameObject prefab;
    public float zPos;

    public string name;
    public Pattern pattern;
    public bool player { get; protected set; }

    public ColorMapping()
    {
        player = false;
        zPos = 0;
    }

    public static Pattern SetTexturePattern()
    {
        Pattern p = new();
        string path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png");
        if (path.Length != 0)
        {
            byte[] fileContent = File.ReadAllBytes(path);
            p = TextureToPattern(fileContent);
        }

        return p;
    }

    private static Pattern TextureToPattern(byte[] texInfo)
    {
        Pattern p = new();
        Texture2D tex = new(0, 0, TextureFormat.RGBA32, false);
        tex.LoadImage(texInfo);

        if (tex.width != 3 && tex.height != 3)
        {
            Debug.LogError("LevelGenerator (ColorMapping): Applied Pattern Texture is not 3x3 size!!");
            return p;
        }

        for (int i = 0; i < 9; i++)
        {
            Color c = tex.GetPixel(i / 3, i % 3);

            if (!c.Equals(new Color(0, 0, 0, 0)) || !c.Equals(new Color(1, 1, 1, 1)))
            {
                Debug.LogWarning(new StringBuilder("LevelGenerator (ColorMapping): Only white color and invisible color are supported while reading pattern from texture. Given Color was ").Append(c).ToString());
            }

            p.Set(i, c.Equals(Color.white));
        }

        return p;
    }
}
