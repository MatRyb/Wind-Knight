using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level Generator Data", menuName = "Level Generator/Level Generator Data")]
public class LevelGeneratorData : ScriptableObject
{
    [Header("Mesh:")]
    public Material insideMeshMaterial;
    [Header("Sprite:")]
    public Material insideSpriteMaterial;
    public Texture2D insideSpriteTexture;
    [Header("Base Settings:")]
    public float roomsZPos;
    [Header("Plane:")]
    public GameObject outsidePlane;
    public float planeZPos;
    [Header("Camera Settings:")]
    public Color cameraBackground = Color.gray;
    public float cameraSize = 14f;
    [Range(0, 10)] public float cameraSmoothFactor = 10f;

    [Header("ColorMappings:")]
    public PlayerColorMapping playerMapping;

    public List<ColorMapping> colorMappings;

    [Header("Managers:")]
    public List<GameObject> additionalObjects;
}
