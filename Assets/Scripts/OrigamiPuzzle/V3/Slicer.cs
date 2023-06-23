using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer
{
    /// <summary>
    /// Slice the object by the plane 
    /// </summary>
    /// <param name="plane"></param>
    /// <param name="objectToCut"></param>
    /// <returns></returns>
    public static GameObject[] Slice(Plane plane, GameObject objectToCut)
    {
        //Get the current mesh and its verts and tris
        Mesh mesh = objectToCut.GetComponent<MeshFilter>().mesh;
        var a = mesh.GetSubMesh(0);
        Sliceable sliceable = objectToCut.GetComponent<Sliceable>();

        //Create left and right slice of hollow object
        SlicesMetadata slicesMeta = new(plane, mesh, sliceable.IsSolid, sliceable.ReverseWireTriangles, sliceable.ShareVertices, sliceable.SmoothVertices);

        GameObject positiveObject = CreateMeshGameObject(objectToCut);
        positiveObject.name = string.Format("{0}_positive", objectToCut.name);

        GameObject negativeObject = CreateMeshGameObject(objectToCut);
        negativeObject.name = string.Format("{0}_negative", objectToCut.name);

        var positiveSideMeshData = slicesMeta.PositiveSideMesh;
        var negativeSideMeshData = slicesMeta.NegativeSideMesh;

        positiveObject.GetComponent<MeshFilter>().mesh = positiveSideMeshData;
        negativeObject.GetComponent<MeshFilter>().mesh = negativeSideMeshData;

        return new GameObject[] { positiveObject, negativeObject };
    }

    /// <summary>
    /// Creates the default mesh game object.
    /// </summary>
    /// <param name="originalObject">The original object.</param>
    /// <returns></returns>
    private static GameObject CreateMeshGameObject(GameObject originalObject)
    {
        var originalMaterial = originalObject.GetComponent<MeshRenderer>().materials;

        GameObject meshGameObject = new();
        Sliceable originalSliceable = originalObject.GetComponent<Sliceable>();

        meshGameObject.AddComponent<MeshFilter>();
        meshGameObject.AddComponent<MeshRenderer>().materials = originalMaterial;
        Sliceable sliceable = meshGameObject.AddComponent<Sliceable>();

        sliceable.IsSolid = originalSliceable.IsSolid;
        sliceable.ReverseWireTriangles = originalSliceable.ReverseWireTriangles;
        sliceable.UseGravity = originalSliceable.UseGravity;

        meshGameObject.transform.parent = originalObject.transform.parent;
        meshGameObject.transform.localScale = originalObject.transform.localScale;
        meshGameObject.transform.SetPositionAndRotation(originalObject.transform.position, originalObject.transform.rotation);

        meshGameObject.tag = originalObject.tag;

        return meshGameObject;
    }
}
