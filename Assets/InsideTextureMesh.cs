using UnityEngine;
using NaughtyAttributes;

public class InsideTextureMesh : MonoBehaviour
{
    [System.Serializable]
    struct Triangle
    {
        public Transform vert1;
        public Transform vert2;
        public Transform vert3;
    }

    private Vector3[] Verticles;
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    [SerializeField] private Material material;
    [SerializeField] private string meshName;
    [SerializeField] private float zPos;
    [Foldout("Info")]
    [DisableIf("true")] [SerializeField] private float height;
    [Foldout("Info")]
    [DisableIf("true")] [SerializeField] private float width;
    [Foldout("Info")]
    [DisableIf("true")] [Tooltip("height / width")] [SerializeField] private float ratio;

    private GameObject renderer = null;

    void Start()
    {
        GenerateMesh();
    }

    public void SetVariables(Material material, string meshName, float zPos)
    {
        this.material = material;
        this.meshName = meshName;
        this.zPos = zPos;
    }

    [Button]
    public void GenerateMesh()
    {
        var transforms = gameObject.GetComponentsInChildren<Transform>();
        Verticles = new Vector3[transforms.Length - 1];
        for (int i = 1; i < transforms.Length; i++)
        {
            Verticles[i - 1] = transforms[i].position;
        }

        int[] triangles = new int[(Verticles.Length / 4) * 6];
        int v = 0;
        for (int t = 0; t < triangles.Length && v < Verticles.Length; v += 4, t += 6)
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
        for (int i = 0; i < Verticles.Length; i++)
        {
            if (Verticles[i].x > maxCoords.x)
            {
                maxCoords.x = Verticles[i].x;
            }

            if (Verticles[i].x < minCoords.x)
            {
                minCoords.x = Verticles[i].x;
            }

            if (Verticles[i].y > maxCoords.y)
            {
                maxCoords.y = Verticles[i].y;
            }

            if (Verticles[i].y < minCoords.y)
            {
                minCoords.y = Verticles[i].y;
            }
        }

        Vector2[] uvs = new Vector2[Verticles.Length];
        for (int i = 0; i < Verticles.Length; i++)
        {
            uvs[i] = new Vector2(Verticles[i].x / maxCoords.x, Verticles[i].y / maxCoords.y);
        }

        renderer = new(meshName);
        renderer.transform.position += new Vector3(0, 0, zPos);
        meshFilter = renderer.AddComponent<MeshFilter>();
        meshRenderer = renderer.AddComponent<MeshRenderer>();

        mesh = new();
        mesh.SetVertices(Verticles);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshRenderer.material = material;

        height = maxCoords.y - minCoords.y;
        width = maxCoords.x - minCoords.x;

        ratio = height / width;
    }

    [Button]
    public void DeleteMesh()
    {
        if (renderer != null)
        {
            DestroyImmediate(renderer);
            height = 0;
            width = 0;
            ratio = 0;
        }
        else
        {
            Debug.Log("Mesh wasn't created!");
        }
    }
}
