using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("levelMap"));
        if (serializedObject.FindProperty("levelMap").objectReferenceValue == null)
        {
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("The level map is the main element on the basis of which the level is generated. Without it, the generator will not work.", MessageType.Error);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            Texture2D myTexture = AssetPreview.GetAssetPreview(serializedObject.FindProperty("levelMap").objectReferenceValue);
            EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(myTexture);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("vertexMap"));
        if (serializedObject.FindProperty("vertexMap").objectReferenceValue == null)
        {
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Without a vertex map, the generator will not create the points needed to generate the level background.", MessageType.Warning);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            Texture2D myTexture = AssetPreview.GetAssetPreview(serializedObject.FindProperty("vertexMap").objectReferenceValue);
            EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(myTexture);
            EditorGUILayout.EndHorizontal();

            if (serializedObject.FindProperty("insideMeshMaterial").objectReferenceValue == null || serializedObject.FindProperty("insideSpriteMaterial").objectReferenceValue == null || serializedObject.FindProperty("insideSpriteTexture").objectReferenceValue == null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("This materials and texture are needed to generate background inside walls. With only a vertex map defined and no materials defined, backgrounds will not be generated.", MessageType.Error);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("insideMeshMaterial"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("insideSpriteMaterial"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("insideSpriteTexture"));
        }

        if (serializedObject.FindProperty("outsidePlane").objectReferenceValue == null)
        {
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Plane outside level won't be generated.", MessageType.Warning);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("outsidePlane"));

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

        GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(serializedObject.FindProperty("vertexMap").objectReferenceValue == null);

                if (GUILayout.Button("Generate Mesh"))
                {
                    generator.GenerateMesh();
                }

                if (GUILayout.Button("Delete Mesh"))
                {
                    generator.DeleteMesh();
                }

            EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();
    }
}
