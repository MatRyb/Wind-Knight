using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level Generator Data", menuName = "Level Generator/Level Generator Data")]
public class LevelGeneratorData : ScriptableObject
{
    public Material meshMaterial;

    public Material spriteMaterial;
    public Texture2D spriteTexture;
    public float roomsZPos;
    public GameObject outsidePlane;
    public float planeZPos;

    public Color cameraBackground = Color.gray;
    public float cameraSize = 14f;
    [Range(0, 10)] public float cameraSmoothFactor = 10f;

    public PlayerColorMapping playerMapping;

    public List<ColorMapping> colorMappings;

    public List<GameObject> additionalObjects;
}
