using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

public class InsideTextureSprite : MonoBehaviour
{
    [System.Serializable]
    struct Triangle
    {
        public Transform vert1;
        public Transform vert2;
        public Transform vert3;
    }

    private Vector3[] vertices;
    private Sprite sprite;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Material material;
    [SerializeField] private Texture2D texture;
    [SerializeField] private string spriteName;
    [SerializeField] private float zPos;
    [Foldout("Info")]
    [DisableIf("true")] [SerializeField] private float height;
    [Foldout("Info")]
    [DisableIf("true")] [SerializeField] private float width;
    [Foldout("Info")]
    [DisableIf("true")] [Tooltip("height / width")] [SerializeField] private float ratio;

    void Start()
    {
        GenerateSprite();
    }

    private Vector3 VertexPointToGlobal(Vector3 point, GameObject meshHandler)
    {
        Matrix4x4 trs = meshHandler.transform.localToWorldMatrix;
        return trs.MultiplyPoint3x4(point);
    }

    private void GetMaxAndMinCoordinates(Vector2[] coordinates, ref Vector2 min, ref Vector2 max)
    {
        min.x = float.PositiveInfinity;
        min.y = float.PositiveInfinity;
        max.x = float.NegativeInfinity;
        max.y = float.NegativeInfinity;
        for (int i = 0; i < coordinates.Length; i++)
        {
            if (coordinates[i].x > max.x)
            {
                max.x = coordinates[i].x;
            }

            if (coordinates[i].y > max.y)
            {
                max.y = coordinates[i].y;
            }

            if (coordinates[i].x < min.x)
            {
                min.x = coordinates[i].x;
            }

            if (coordinates[i].y < min.y)
            {
                min.y = coordinates[i].y;
            }
        }
    }

    private void GenerateSprite()
    {
        var transforms = gameObject.GetComponentsInChildren<Transform>();
        vertices = new Vector3[transforms.Length - 1];
        for (int i = 1; i < transforms.Length; i++)
        {
            vertices[i - 1] = transforms[i].position;
        }

        int[] triangles = new int[(vertices.Length / 4) * 6];
        int v = 0;
        for (int t = 0; t < triangles.Length && v < vertices.Length; v += 4, t += 6)
        {
            triangles[t] = v;
            triangles[t + 1] = v + 1;
            triangles[t + 2] = v + 2;
            triangles[t + 3] = v + 2;
            triangles[t + 4] = v + 1;
            triangles[t + 5] = v + 3;
        }

        Vector2 maxCoords = Vector2.zero;
        Vector2 minCoords = Vector2.zero;
        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i].x > maxCoords.x)
            {
                maxCoords.x = vertices[i].x;
            }

            if (vertices[i].x < minCoords.x)
            {
                minCoords.x = vertices[i].x;
            }

            if (vertices[i].y > maxCoords.y)
            {
                maxCoords.y = vertices[i].y;
            }

            if (vertices[i].y < minCoords.y)
            {
                minCoords.y = vertices[i].y;
            }
        }

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x / maxCoords.x, vertices[i].y / maxCoords.y);
        }

        GameObject renderer = new(spriteName);
        renderer.transform.position += new Vector3(0, 0, zPos);
        spriteRenderer = renderer.AddComponent<SpriteRenderer>();

        sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        spriteRenderer.sprite = sprite;

        sprite = spriteRenderer.sprite;

        List<Vector3> normalizedVertices = new();

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 normalized = new((vertices[i].x - minCoords.x) / (maxCoords.x - minCoords.x), (vertices[i].y - minCoords.y) / (maxCoords.y - minCoords.y));
            normalizedVertices.Add(normalized);
        }

        MeshToSprite.ConvertMeshsToSprite(normalizedVertices.ToArray(), triangles, ref sprite);

        spriteRenderer.sprite = sprite;

        // Scale Sprite
        Vector2 spriteMin = Vector2.zero;
        Vector2 spriteMax = Vector2.zero;
        GetMaxAndMinCoordinates(sprite.vertices, ref spriteMin, ref spriteMax);
        spriteMin = VertexPointToGlobal(spriteMin, spriteRenderer.gameObject);
        spriteMax = VertexPointToGlobal(spriteMax, spriteRenderer.gameObject);

        float widthRatio = (maxCoords.x - minCoords.x) / (spriteMax.x - spriteMin.x);
        float heightRatio = (maxCoords.y - minCoords.y) / (spriteMax.y - spriteMin.y);

        spriteRenderer.gameObject.transform.localScale = new Vector3(spriteRenderer.gameObject.transform.localScale.x * widthRatio, spriteRenderer.gameObject.transform.localScale.y * heightRatio, 1f);

        // Transform Sprite
        GetMaxAndMinCoordinates(sprite.vertices, ref spriteMin, ref spriteMax);
        spriteMin = VertexPointToGlobal(spriteMin, spriteRenderer.gameObject);
        spriteMax = VertexPointToGlobal(spriteMax, spriteRenderer.gameObject);

        spriteRenderer.gameObject.transform.position += new Vector3(maxCoords.x - spriteMax.x, maxCoords.y - spriteMax.y, 0f);

        spriteRenderer.material = material;
        spriteRenderer.sortingLayerName = "Background";

        height = maxCoords.y - minCoords.y;
        width = maxCoords.x - minCoords.x;

        ratio = height / width;
    }
}
