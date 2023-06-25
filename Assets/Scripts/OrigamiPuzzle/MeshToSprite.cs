using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshToSprite
{
    public static void ConvertMeshsToSprite(Vector3[] meshVertices, int[] meshTriangles, ref Sprite sprite)
    {
        List<Vector2> verticies = new();
        List<ushort> triangles = new();
        List<(int, int)> vericiesNewIndexes = new();
        for (int v = 0; v < meshVertices.Length; v++)
        {
            Vector2 convertedVertice = new(meshVertices[v].x * (sprite.rect.xMax - sprite.rect.xMin) + sprite.rect.xMin, meshVertices[v].y * (sprite.rect.yMax - sprite.rect.yMin) + sprite.rect.yMin);

            if (!verticies.Exists((vert) => vert.x == convertedVertice.x && vert.y == convertedVertice.y))
            {
                verticies.Add(convertedVertice);
                if (verticies.Count - 1 != v)
                {
                    vericiesNewIndexes.Add((v, verticies.Count - 1));
                }
            }
            else
            {
                vericiesNewIndexes.Add((v, verticies.FindIndex((vert) => vert.x == convertedVertice.x && vert.y == convertedVertice.y)));
            }
        }

        for (int t = 0; t < meshTriangles.Length; t++)
        {
            if (!vericiesNewIndexes.Exists((d) => d.Item1.Equals(meshTriangles[t])))
            {
                triangles.Add((ushort)meshTriangles[t]);
            }
            else
            {
                triangles.Add((ushort)vericiesNewIndexes.Find((d) => d.Item1.Equals(meshTriangles[t])).Item2);
            }
        }

        sprite.OverrideGeometry(verticies.ToArray(), triangles.ToArray());
    }
}
