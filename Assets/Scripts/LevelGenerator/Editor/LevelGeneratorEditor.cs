using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("levelMap"));
        Texture2D myTexture = AssetPreview.GetAssetPreview(serializedObject.FindProperty("levelMap").objectReferenceValue);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(myTexture);
        GUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("vertexMap"));
        myTexture = AssetPreview.GetAssetPreview(serializedObject.FindProperty("vertexMap").objectReferenceValue);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(myTexture);
        GUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("colorMappings"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("objectsCount"));
        serializedObject.ApplyModifiedProperties();

        LevelGenerator generator = (LevelGenerator)target;

        GUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate Level"))
            {
                generator.GenerateLevel();
            }

            if (GUILayout.Button("Delete Level"))
            {
                generator.DeleteLevel();
            }

        GUILayout.EndHorizontal();
    }
}
