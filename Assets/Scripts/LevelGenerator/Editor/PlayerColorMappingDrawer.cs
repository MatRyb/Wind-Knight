using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PlayerColorMapping))]
public class PlayerColorMappingDrawer : PropertyDrawer
{
	readonly float propertyHeight = 20f;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		int oldIndentLevel = EditorGUI.indentLevel;
		label = EditorGUI.BeginProperty(position, label, property);

		Rect contentPosition = EditorGUI.PrefixLabel(position, label);

		if (position.height - 28f > propertyHeight * 6f)
		{
			position.height = propertyHeight;
			EditorGUI.indentLevel += 1;
			contentPosition = EditorGUI.IndentedRect(position);
			contentPosition.y += propertyHeight + 2f;
		}

		contentPosition.height = propertyHeight;
		EditorGUI.indentLevel = 0;
		//contentPosition.x += contentPosition.width + 5f;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("color"), GUIContent.none);
		contentPosition.y += propertyHeight + 3f;
		contentPosition.height = propertyHeight + 2f;
		EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("prefab"), GUIContent.none);

		contentPosition.y += propertyHeight + 9f;
		SerializedProperty p = property.FindPropertyRelative("pattern");
		EditorGUI.PropertyField(contentPosition, p, new GUIContent("Pattern"));
		contentPosition.y += propertyHeight * 3 + 12f;
		contentPosition.height = propertyHeight + 2f;

		if (GUI.Button(contentPosition, "Set Pattern From Texture"))
		{
			SetPattern(ref p);
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
		return (label != GUIContent.none && Screen.width < 333 ? propertyHeight * 7f + 28f : propertyHeight * 6f + 26f);
	}
}
