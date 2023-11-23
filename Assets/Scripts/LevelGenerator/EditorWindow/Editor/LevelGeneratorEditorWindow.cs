using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class LevelGeneratorEditorWindow : EditorWindow
{
    private Texture2D levelMap;
    private Texture2D vertexMap;
    [Header("Mesh:")]
    private Material insideMeshMaterial;
    [Header("Sprite:")]
    private Material insideSpriteMaterial;
    private Texture2D insideSpriteTexture;
    private LevelGeneratorData _data;

    [MenuItem("Window/Level Generator")]
    public static void ShowWindow()
    {
        GetWindow<LevelGeneratorEditorWindow>("Level Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Levels Fast ");

        levelMap = (Texture2D)EditorGUILayout.ObjectField("Level Map", levelMap, typeof(Texture2D), false);
        if (levelMap == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("The level map is the main element on the basis of which the level is generated. Without it, the generator will not work.", MessageType.Error);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            Texture2D myTexture = AssetPreview.GetAssetPreview(levelMap);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(myTexture);
            EditorGUILayout.EndHorizontal();
        }

        vertexMap = (Texture2D)EditorGUILayout.ObjectField("Vertex Map", vertexMap, typeof(Texture2D), false);

        if (vertexMap == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Without a vertex map, the generator will not create the points needed to generate the level background.", MessageType.Warning);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            Texture2D myTexture = AssetPreview.GetAssetPreview(vertexMap);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(myTexture);
            EditorGUILayout.EndHorizontal();

            if (insideMeshMaterial == null || insideSpriteMaterial == null || insideSpriteTexture == null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("This materials and texture are needed to generate background inside walls. With only a vertex map defined and no materials defined, backgrounds will not be generated.", MessageType.Error);
                EditorGUILayout.EndHorizontal();
            }
            insideMeshMaterial = (Material)EditorGUILayout.ObjectField("Inside Mesh Material", insideMeshMaterial, typeof(Material), false);
            insideSpriteMaterial = (Material)EditorGUILayout.ObjectField("Inside Sprite Material", insideSpriteMaterial, typeof(Material), false);
            insideSpriteTexture = (Texture2D)EditorGUILayout.ObjectField("Inside Sprite Texture", insideSpriteTexture, typeof(Texture2D), false);
        }
    }
}
