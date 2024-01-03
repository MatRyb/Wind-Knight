using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class AssetHandler
{
    [OnOpenAsset()]
    public static bool OpenEditor(int instanceId, int line)
    {
        LevelGeneratorData obj = EditorUtility.InstanceIDToObject(instanceId) as LevelGeneratorData;
        if (obj != null)
        {
            LevelGeneratorDataEditorWindow.Open(obj);
            return true;
        }
        return false;
    }
}

[CustomEditor(typeof(LevelGeneratorData))]
public class LevelGeneratorDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Editor"))
        {
            LevelGeneratorDataEditorWindow.Open((LevelGeneratorData)target);
        }
    }
}
