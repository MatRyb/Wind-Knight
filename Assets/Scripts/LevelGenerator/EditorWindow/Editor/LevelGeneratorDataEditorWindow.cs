using UnityEngine;
using UnityEditor;

public class LevelGeneratorDataEditorWindow : ExtendedEditorWindow
{
    private Vector2 scrollPos;

    public static void Open(LevelGeneratorData generatorData)
    {
        LevelGeneratorDataEditorWindow window = GetWindow<LevelGeneratorDataEditorWindow>("Level Generator Data Editor");
        window.serializedObject = new SerializedObject(generatorData);
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("insideMeshMaterial"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("insideSpriteMaterial"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("insideSpriteTexture"));
        if (serializedObject.FindProperty("insideMeshMaterial").objectReferenceValue == null ||
            serializedObject.FindProperty("insideSpriteMaterial").objectReferenceValue == null ||
            serializedObject.FindProperty("insideSpriteTexture").objectReferenceValue == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("This materials and texture are needed to generate background inside walls. With only a vertex map defined and no materials defined, backgrounds will not be generated.", MessageType.Error);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("roomsZPos"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("outsidePlane"));
        if (serializedObject.FindProperty("outsidePlane").objectReferenceValue == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Plane outside level won't be generated.", MessageType.Warning);
            EditorGUILayout.EndHorizontal();
        }
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
        EditorGUILayout.EndScrollView();
        serializedObject.ApplyModifiedProperties();
    }
}
