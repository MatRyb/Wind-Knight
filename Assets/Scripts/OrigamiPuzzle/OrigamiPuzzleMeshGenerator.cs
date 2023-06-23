using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OrigamiPuzzleMeshGenerator
{
    public static Mesh GeneratePaperMesh(int width, int height)
    {
        int vertexCount = (width + 1) * (height + 1);
        if (vertexCount < 1 || width < 1 || height < 1) throw new System.Exception();

        // Create Vertices
        Vector3 startPos = new(-(float)width / 2f, -(float)height / 2f, 0);

        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        for (int i = 0, h = 0; h <= height; h++)
        {
            for (int w = 0; w <= width; w++)
            {
                vertices[i] = startPos + new Vector3(w, h, 0);
                uvs[i] = new Vector2((float)w / (float)width, (float)h / (float)height);
                i++;
            }
        }

        int [] triangles = new int[width * height * 6];

        int vert = 0;
        int tris = 0;
        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        Mesh mesh = new();

        mesh.name = "Origami Puzzle Mesh";
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}
