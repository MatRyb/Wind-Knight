using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level Generator Data", menuName = "Level Generator/Level Generator Data")]
public class LevelGeneratorData : ScriptableObject
{
    [Header("Base Settings:")]
    [SerializeField] private float roomsZPos;
    [Header("Plane:")]
    [SerializeField] private GameObject outsidePlane;
    [SerializeField] private float planeZPos;
    [Header("Camera Settings:")]
    [SerializeField] private Color cameraBackground = Color.gray;
    [SerializeField] private float cameraSize = 14f;
    [SerializeField][Range(0, 10)] private float cameraSmoothFactor = 10f;

    [Header("ColorMappings:")]
    [SerializeField] private PlayerColorMapping playerMapping;

    [SerializeField] private List<ColorMapping> colorMappings;

    [Header("Managers:")]
    [SerializeField] private List<GameObject> additionalObjects;
}
