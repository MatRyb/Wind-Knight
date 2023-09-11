using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("map"));
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
