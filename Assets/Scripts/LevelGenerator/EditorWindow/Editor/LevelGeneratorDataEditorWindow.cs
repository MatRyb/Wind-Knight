using UnityEngine;
using UnityEditor;
using System.ComponentModel;
using System.Reflection;
using UnityEditorInternal;
using System.Net.Configuration;

enum LevelGeneratorDataCategory
{
    [Description("Vertex Map")] VERTEX_MAP = 0,
    [Description("Object Settings")] OBJECTS_SETTINGS = 1,
    [Description("Camera")] CAMERA = 2,
    [Description("Color Mappings")] COLOR_MAPPINGS = 3,
    [Description("Additional Objects")] ADDITIONAL_OBJECTS = 4
}

public class LevelGeneratorDataEditorWindow : ExtendedEditorWindow
{
    private Vector2 sidebarScrollPos;
    private Vector2 viewportScrollPos;

    private LevelGeneratorDataCategory category;

    private LevelGeneratorData temp;

    private bool initDone = false;
    private GUIStyle TitleLabel;
    private GUIStyle CategoryLabel;
    private GUIStyle HeaderLabel;

    ReorderableList l;

    [MenuItem("Window/Level Generator/Data Editor")]
    public static void ShowWindow()
    {
        Open(null);
    }

    public static void Open(LevelGeneratorData generatorData)
    {
        LevelGeneratorDataEditorWindow window = GetWindow<LevelGeneratorDataEditorWindow>("Level Generator Data Editor");
        if (generatorData == null)
        {
            window.serializedObject = null;
        }
        else
        {
            window.serializedObject = new SerializedObject(generatorData);

            window.l = new(window.serializedObject, window.serializedObject.FindProperty("additionalObjects"), true, false, true, true);

            window.l.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2.4f;
                rect.height = EditorGUIUtility.singleLineHeight;

                GUIContent objectLabel = new($"Element {index}");

                EditorGUI.PropertyField(rect, window.l.serializedProperty.GetArrayElementAtIndex(index), objectLabel);
            };
        }
    }

    void InitStyles()
    {
        TitleLabel = new GUIStyle(EditorStyles.largeLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };

        CategoryLabel = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 13,
            fontStyle = FontStyle.Italic
        };

        HeaderLabel = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = 12,
            fontStyle = FontStyle.Bold
        };

        initDone = true;
    }

    private void OnGUI()
    {
        if (!initDone)
            InitStyles();

        if (serializedObject != null)
        {
            EditorGUILayout.BeginVertical("box");

            GUILayout.Label(serializedObject.targetObject.name, TitleLabel);
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical("box", GUILayout.Width(150));
            sidebarScrollPos = EditorGUILayout.BeginScrollView(sidebarScrollPos);

            GUILayout.Label("Categories", CategoryLabel);

            foreach (System.Enum c in System.Enum.GetValues(typeof(LevelGeneratorDataCategory)))
            {
                if (GUILayout.Button(GetDescription(c)))
                {
                    category = (LevelGeneratorDataCategory)c;
                    viewportScrollPos = Vector2.zero;
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            Rect r = EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(false));
            viewportScrollPos = EditorGUILayout.BeginScrollView(viewportScrollPos);

            switch (category)
            {
                case LevelGeneratorDataCategory.VERTEX_MAP:
                    {
                        GUILayout.Label("Mesh:", HeaderLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("meshMaterial"));
                        GUILayout.Space(10);
                        GUILayout.Label("Sprite:", HeaderLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("spriteMaterial"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("spriteTexture"));
                        if (serializedObject.FindProperty("meshMaterial").objectReferenceValue == null ||
                            serializedObject.FindProperty("spriteMaterial").objectReferenceValue == null ||
                            serializedObject.FindProperty("spriteTexture").objectReferenceValue == null)
                        {
                            EditorGUILayout.HelpBox("This materials and texture are needed to generate background inside walls. With only a vertex map defined and no materials defined, backgrounds will not be generated.", MessageType.Error);
                        }
                        break;
                    }
                case LevelGeneratorDataCategory.OBJECTS_SETTINGS:
                    {
                        GUILayout.Label("Basic Settings:", HeaderLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("roomsZPos"));
                        GUILayout.Space(10);
                        GUILayout.Label("Plane:", HeaderLabel);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("outsidePlane"));
                        if (serializedObject.FindProperty("outsidePlane").objectReferenceValue == null)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("Plane outside level won't be generated.", MessageType.Warning);
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("planeZPos"));
                        break;
                    }
                case LevelGeneratorDataCategory.CAMERA:
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraBackground"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraSize"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraSmoothFactor"));
                        break;
                    }
                case LevelGeneratorDataCategory.COLOR_MAPPINGS:
                    {
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
                        break;
                    }
                case LevelGeneratorDataCategory.ADDITIONAL_OBJECTS:
                    {
                        l.DoList(new Rect(Vector2.zero, new Vector2(r.width - 10, r.height)));
                        //EditorGUILayout.PropertyField(serializedObject.FindProperty("additionalObjects"));
                        break;
                    }
                default:
                    {
                        EditorGUILayout.TextArea("This is Editor for Level Generator Data. I hope it will help you design levels :)");
                        break;
                    }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
        else
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Level Generator Data Editor", TitleLabel);
            GUILayout.Space(5);
            temp = (LevelGeneratorData)EditorGUILayout.ObjectField("Level Generator Data", temp, typeof(LevelGeneratorData), false);
            EditorGUILayout.Space(5);

            GUI.enabled = temp != null;

            if (GUILayout.Button("Load Data"))
            {
                Open(temp);
                /*serializedObject = new SerializedObject(temp);
                category = LevelGeneratorDataCategory.VERTEX_MAP;
                temp = null;*/
            }
            EditorGUILayout.Space(10);
            EditorGUILayout.EndVertical();
        }
    }

    private static string GetDescription(System.Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes.Length > 0)
            return attributes[0].Description;
        else
            return value.ToString();
    }
}
