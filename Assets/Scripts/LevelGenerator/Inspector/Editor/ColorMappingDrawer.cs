using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ColorMapping))]
public class ColorMappingDrawer : PropertyDrawer
{
    readonly float propertyHeight = 20f;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		int oldIndentLevel = EditorGUI.indentLevel;
		bool pattern = (MappingType)property.FindPropertyRelative("type").intValue == MappingType.Pattern;
		label = EditorGUI.BeginProperty(position, label, property);

			Rect contentPosition = EditorGUI.PrefixLabel(position, label);

			if ((pattern && position.height - 27f > propertyHeight * 9.5f) || (!pattern && position.height - 14f > propertyHeight * 3))
			{
				position.height = propertyHeight;
				EditorGUI.indentLevel += 1;
				contentPosition = EditorGUI.IndentedRect(position);
				contentPosition.y += propertyHeight + 2f;
			}

			contentPosition.height = propertyHeight;
			contentPosition.width *= 0.5f;
			EditorGUI.indentLevel = 0;
			var type = property.FindPropertyRelative("type");
			type.intValue = EditorGUI.Popup(contentPosition, "", type.intValue, type.enumNames);
			switch ((MappingType)type.intValue)
			{
				case MappingType.Pattern:
					pattern = true;
					break;
				default:
					pattern = false;
					break;
			}
			//EditorGUI.PropertyField(contentPosition, type, GUIContent.none);
			contentPosition.x += contentPosition.width + 5f;
			contentPosition.width /= 1.03f;
			contentPosition.height = propertyHeight - 3f;
			EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("color"), GUIContent.none);
			contentPosition.y += propertyHeight + 3f;
			contentPosition.x -= contentPosition.width + 5f;
			contentPosition.height = propertyHeight + 2f;
			contentPosition.width *= 1.03f * 2f;
			EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("prefab"), GUIContent.none);
			contentPosition.y += propertyHeight + 3f;
			EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("zPos"), new GUIContent("Z Pos"));

		if (pattern)
			{
				contentPosition.y += propertyHeight + 3f;
				EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("name"), new GUIContent("Name"));
				contentPosition.y += propertyHeight + 9f;
				SerializedProperty p = property.FindPropertyRelative("pattern");
				EditorGUI.PropertyField(contentPosition, p, new GUIContent("Pattern"));
				contentPosition.y += propertyHeight * 3 + 12f;
				contentPosition.height = propertyHeight + 2f;

				if (GUI.Button(contentPosition, "Set Pattern From Texture"))
				{
					SetPattern(ref p);
				}
			}


		EditorGUI.EndProperty();
		EditorGUI.indentLevel = oldIndentLevel;
	}

	private void SetPattern(ref SerializedProperty patternObj)
    {
		Pattern p = ColorMapping.SetTexturePattern();
		patternObj.FindPropertyRelative("ul").boolValue = p.ul;
		patternObj.FindPropertyRelative("um").boolValue = p.um;
		patternObj.FindPropertyRelative("ur").boolValue = p.ur;
		patternObj.FindPropertyRelative("ml").boolValue = p.ml;
		patternObj.FindPropertyRelative("mm").boolValue = p.mm;
		patternObj.FindPropertyRelative("mr").boolValue = p.mr;
		patternObj.FindPropertyRelative("dl").boolValue = p.dl;
		patternObj.FindPropertyRelative("dm").boolValue = p.dm;
		patternObj.FindPropertyRelative("dr").boolValue = p.dr;
		patternObj.serializedObject.ApplyModifiedProperties();
		GUIUtility.ExitGUI();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return (label != GUIContent.none && Screen.width < 333 ? ((MappingType)property.FindPropertyRelative("type").intValue == MappingType.Pattern ? propertyHeight * 9.5f + 27f : propertyHeight * 4 + 12f) : ((MappingType)property.FindPropertyRelative("type").intValue == MappingType.Pattern ? propertyHeight * 9.5f + 13f : propertyHeight * 3 + 12f));
	}
}
