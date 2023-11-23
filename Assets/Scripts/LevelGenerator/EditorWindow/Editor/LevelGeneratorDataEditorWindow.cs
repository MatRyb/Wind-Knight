using UnityEngine;
using UnityEditor;

public class LevelGeneratorDataEditorWindow : ExtendedEditorWindow
{
    public static void Open(LevelGeneratorData generatorData)
    {
        LevelGeneratorDataEditorWindow window = GetWindow<LevelGeneratorDataEditorWindow>("Level Generator Data Editor");
        window.serializedObject = new SerializedObject(generatorData);
    }

    private void OnGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("roomsZPos"));

        if (serializedObject.FindProperty("outsidePlane").objectReferenceValue == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Plane outside level won't be generated.", MessageType.Warning);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("outsidePlane"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("planeZPos"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraBackground"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraSize"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraSmoothFactor"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerMapping"));
        if (serializedObject.FindProperty("playerMapping").FindPropertyRelative("prefab").objectReferenceValue == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("You need to define Player prefab if you want to generate Player.", MessageType.Warning);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.HelpBox("Checking which object type will be used for a given pixel color is in the order: \n Pattern > Wall (Including Angel Wall) > Ground", MessageType.Info);
        if (serializedObject.FindProperty("colorMappings").arraySize == 0)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("There need to be at least one color mapping defined.", MessageType.Error);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("colorMappings"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("additionalObjects"));

        serializedObject.ApplyModifiedProperties();
    }
}
