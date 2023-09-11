using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Pattern))]
public class PatternDrawer : PropertyDrawer
{
    readonly float propertySize = 20f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int oldIndentLevel = EditorGUI.indentLevel;
        label = EditorGUI.BeginProperty(position, label, property);

            Rect contentPosition = EditorGUI.PrefixLabel(position, label);
            EditorGUI.indentLevel = 0;
            contentPosition.height = propertySize;
            contentPosition.width = propertySize;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("ul"), GUIContent.none);
            contentPosition.x += contentPosition.width + 5f;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("um"), GUIContent.none);
            contentPosition.x += contentPosition.width + 5f;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("ur"), GUIContent.none);
            contentPosition.x -= 2*(contentPosition.width + 5f);
            contentPosition.y += contentPosition.height + 5f;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("ml"), GUIContent.none);
            contentPosition.x += contentPosition.width + 5f;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("mm"), GUIContent.none);
            contentPosition.x += contentPosition.width + 5f;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("mr"), GUIContent.none);
            contentPosition.x -= 2 * (contentPosition.width + 5f);
            contentPosition.y += contentPosition.height + 5f;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("dl"), GUIContent.none);
            contentPosition.x += contentPosition.width + 5f;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("dm"), GUIContent.none);
            contentPosition.x += contentPosition.width + 5f;
            EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("dr"), GUIContent.none);

        EditorGUI.EndProperty();
        EditorGUI.indentLevel = oldIndentLevel;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return propertySize * 3 + 12f;
    }
}
