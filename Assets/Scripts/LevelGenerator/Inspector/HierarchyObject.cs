using UnityEngine;

public class HierarchyObject
{
    public string path;
    public GameObject referenceObject;

    public HierarchyObject(string path, GameObject referenceObject)
    {
        this.path = path;
        this.referenceObject = referenceObject;
    }
}
