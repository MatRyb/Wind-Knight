using UnityEngine;

public enum MappingType
{
    Ground = 0,
    Wall = 1,
    Pattern = 2
}

public interface IColorMapping
{
    Color color { get; set; }
    MappingType type { get; set; }
    GameObject prefab { get; set; }
    string name { get; set; }
    Pattern pattern { get; set; }
}
