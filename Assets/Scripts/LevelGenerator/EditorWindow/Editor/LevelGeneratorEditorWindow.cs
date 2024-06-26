using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using static UnityEditor.Progress;

public class LevelGeneratorEditorWindow : EditorWindow
{
    private Vector2 scrollPos;
    private LevelGeneratorData _data;
    private Texture2D levelMap;
    private Texture2D vertexMap;

    private List<int> visittedPixels;
    private List<HierarchyObject> hierarchyObjects;

    [MenuItem("Window/Level Generator/Generator")]
    public static void ShowWindow()
    {
        GetWindow<LevelGeneratorEditorWindow>("Level Generator");
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        GUILayout.Label("Create Levels Fast With Generator");
        _data = (LevelGeneratorData)EditorGUILayout.ObjectField("Level Generator Data", _data, typeof(LevelGeneratorData), false);
        levelMap = (Texture2D)EditorGUILayout.ObjectField("Level Map", levelMap, typeof(Texture2D), false);
        if (levelMap == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("The level map is the main element on the basis of which the level is generated. Without it, the generator will not work.", MessageType.Error);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            Texture2D myTexture = AssetPreview.GetAssetPreview(levelMap);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(myTexture);
            EditorGUILayout.EndHorizontal();
        }

        vertexMap = (Texture2D)EditorGUILayout.ObjectField("Vertex Map", vertexMap, typeof(Texture2D), false);

        if (vertexMap == null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("Without a vertex map, the generator will not create the points needed to generate the level background.", MessageType.Warning);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            Texture2D myTexture = AssetPreview.GetAssetPreview(vertexMap);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(myTexture);
            EditorGUILayout.EndHorizontal();
        }

        GUI.enabled = _data != null && levelMap != null;

        if (GUILayout.Button("Generate Level"))
        {
            GenerateLevel();
        }
        EditorGUILayout.Space(10);
        EditorGUILayout.EndScrollView();
    }

    private void GenerateLevel()
    {
        if (levelMap == null)
        {
            Debug.LogError("LevelGenerator: Level Map Texture is null. Please provide one.");
            return;
        }

        if (_data.colorMappings.Count == 0)
        {
            Debug.LogError("LevelGenerator: No ColorMappings were defined. Please define one.");
            return;
        }

        int allPixels = levelMap.width * levelMap.height;
        int middle = allPixels / 2;
        Vector2 middlePos = new(middle / levelMap.height, middle / levelMap.height);

        hierarchyObjects?.Clear();
        hierarchyObjects = new();

        int roomNumber = 0;
        if (GameObject.Find("Rooms") != null)
        {
            hierarchyObjects.Add(new("Rooms", GameObject.Find("Rooms")));
            roomNumber = GameObject.Find("Rooms").transform.childCount > 0 ? GameObject.Find("Rooms").transform.childCount : 0;
        }
        else
        {
            new GameObject("Rooms");
            hierarchyObjects.Add(new("Rooms", GameObject.Find("Rooms")));
        }

        GameObject obj = new("Room " + roomNumber);
        obj.transform.parent = hierarchyObjects.Find(x => x.path == "Rooms").referenceObject.transform;
        hierarchyObjects.Add(new("Room", GameObject.Find("Room " + roomNumber)));

        visittedPixels?.Clear();
        visittedPixels = new();

        List<ColorMapping> sorted = _data.colorMappings.OrderByDescending(c => c, new ColorMappingComparator()).Distinct().ToList();

        if (_data.playerMapping.prefab != null)
        {
            sorted.Insert(0, _data.playerMapping);
        }

        for (int i = 0; i < allPixels; i++)
        {
            if (visittedPixels.Contains(i))
            {
                continue;
            }

            int x = i / levelMap.height;
            int y = i % levelMap.height;
            GenerateTile(x, y, sorted);
        }

        if (GameObject.Find("Textures") != null)
        {
            hierarchyObjects.Add(new("Textures", GameObject.Find("Textures")));
        }
        else
        {
            new GameObject("Textures");
            hierarchyObjects.Add(new("Textures", GameObject.Find("Textures")));
        }

        if (_data.outsidePlane != null)
        {
            if (GameObject.Find("OutsideTexture") != null)
            {
                hierarchyObjects.Add(new("OutsideTexture", GameObject.Find("OutsideTexture")));
            }
            else
            {
                GameObject o = (GameObject)PrefabUtility.InstantiatePrefab(_data.outsidePlane);
                o.transform.parent = hierarchyObjects.Find(x => x.path == "Textures").referenceObject.transform;
                o.transform.position = new Vector3(middlePos.x, middlePos.y, _data.planeZPos);
                o.name = "OutsideTexture";
                hierarchyObjects.Add(new("OutsideTexture", GameObject.Find("OutsideTexture")));
            }
        }

        if (vertexMap != null)
        {
            GameObject vertex = VertexDecoder.DecodeVertexMap(vertexMap);
            vertex.transform.parent = hierarchyObjects.Find(x => x.path == "Textures").referenceObject.transform;
            vertex.AddComponent<InsideTextureMesh>();
            vertex.GetComponent<InsideTextureMesh>().SetVariables(_data.meshMaterial, "Cover", 1);
            vertex.AddComponent<InsideTextureSprite>();
            vertex.GetComponent<InsideTextureSprite>().SetVariables(_data.spriteMaterial, _data.spriteTexture, "Background", 1);
            hierarchyObjects.Add(new("MeshVertex", vertex));
        }

        if (_data.playerMapping.prefab != null)
        {
            MoveEverythingBasedOnPlayer();
        }
        else
        {
            MoveEverythingBasedOnMiddlePoint(middlePos);
        }

        Transform t = hierarchyObjects.Find(x => x.path == "Rooms").referenceObject.transform;
        t.position = new(t.position.x, t.position.y, _data.roomsZPos);

        AddCamera();

        AddGameTimer();

        if (_data.additionalObjects.Count != 0)
        {
            AddAdditionalObjects();
        }

        GenerateMesh();
    }

    private void GenerateMesh()
    {
        if (GameObject.Find("Textures") == null || GameObject.Find("Textures").transform.Find("MeshVertex") == null || GameObject.Find("Textures").transform.Find("MeshVertex").GetComponent<InsideTextureMesh>() == null)
        {
            Debug.LogWarning("MeshVertex wasn't generated");
            return;
        }

        GameObject.Find("Textures").transform.Find("MeshVertex").GetComponent<InsideTextureMesh>().GenerateMesh();
    }

    private void GenerateTile(int x, int y, List<ColorMapping> mappings)
    {
        Color pixelColor = levelMap.GetPixel(x, y);

        if (pixelColor.a == 0)
        {
            // The pixel is transParent ;)
            visittedPixels.Add(x * levelMap.height + y);
            return;
        }

        bool valid = false;

        foreach (ColorMapping colorMapping in mappings)
        {
            if (colorMapping.color.Equals(pixelColor))
            {
                switch (colorMapping.type)
                {
                    case MappingType.Pattern:
                        valid = CheckPattern(colorMapping, x, y);
                        break;
                    case MappingType.Wall:
                        {
                            if (IsGroundDefinied(mappings.IndexOf(colorMapping), mappings))
                            {
                                valid = CheckWallGround(colorMapping, mappings[mappings.IndexOf(colorMapping) + 1], x, y);

                                if (valid)
                                {
                                    break;
                                }
                            }

                            if (CheckWall(colorMapping, x, y))
                            {
                                valid = true;
                                break;
                            }

                            valid = AngelWall(colorMapping, x, y);
                            break;
                        }
                    case MappingType.Ground:
                        valid = CheckGround(colorMapping, x, y);
                        break;
                    default:
                        {
                            Vector2 position = new(x, y);
                            GameObject o = (GameObject)PrefabUtility.InstantiatePrefab(colorMapping.prefab);
                            o.transform.position = position;
                            o.transform.rotation = Quaternion.identity;
                            break;
                        }
                }

                if (valid)
                {
                    valid = false;
                    return;
                }
            }
        }
    }

    private bool IsGroundDefinied(int index, List<ColorMapping> mappings)
    {
        return index + 1 < mappings.Count && mappings[index + 1].color.Equals(mappings[index].color) && mappings[index + 1].type == MappingType.Ground;
    }

    private bool CheckPattern(ColorMapping mapping, int x, int y)
    {
        // x, y - reference position
        // mapping - from this we get info about pattern and color
        // map.height - it is needed to calculate true index of pixel
        //
        // We check where first point could be in pattern which was found in image.

        int reference = 0;

        for (int i = 0; i < 9; i++)
        {
            if (mapping.pattern.Get(i))
            {
                reference = i;
                break;
            }
        }

        int ref_x = reference / 3;
        int ref_y = reference % 3;

        // (0,2) (1,2) (2,2)
        // (0,1) (1,1) (2,1)
        // (0,0) (1,0) (2,0)

        int beg_x = x - ref_x;
        int beg_y = y - ref_y;

        if (beg_x < 0 || beg_y < 0)
        {
            return false;
        }

        if (beg_x + 2f >= levelMap.width || beg_y + 2f >= levelMap.height)
        {
            return false;
        }

        List<(int, int)> patternCheck = new()
        {
            (x, y)
        };

        for (int i = 0; i < 9; i++)
        {
            (int, int) curr_pos = (i / 3, i % 3);

            if (curr_pos.Item1 == ref_x && curr_pos.Item2 == ref_y)
            {
                continue;
            }

            if (mapping.pattern.Get(i))
            {
                if (mapping.color.Equals(levelMap.GetPixel(beg_x + curr_pos.Item1, beg_y + curr_pos.Item2)))
                {
                    patternCheck.Add((beg_x + curr_pos.Item1, beg_y + curr_pos.Item2));
                }
                else
                {
                    return false;
                }
            }
        }

        if (mapping.player)
        {
            Vector3 position = new(beg_x + 1, beg_y + 1, mapping.zPos);
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(mapping.prefab);
            obj.transform.position = position;
            obj.transform.rotation = Quaternion.identity;
            obj.name = "Player";
            if (hierarchyObjects.Find(x => x.path == "Player") == null)
            {
                hierarchyObjects.Add(new("Player", obj));
            }
        }
        else
        {
            if (hierarchyObjects.Find(x => x.path == mapping.name + "s") == null)
            {
                GameObject g = new(mapping.name + "s");
                g.transform.parent = hierarchyObjects.Find(x => x.path == "Room").referenceObject.transform;
                g.transform.position = new(0, 0);
                hierarchyObjects.Add(new(mapping.name + "s", hierarchyObjects.Find(x => x.path == "Room").referenceObject.transform.Find(mapping.name + "s").gameObject));
            }

            Vector3 position = new(beg_x + 1, beg_y + 1, mapping.zPos);
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(mapping.prefab);
            obj.transform.position = position;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.parent = hierarchyObjects.Find(x => x.path == mapping.name + "s").referenceObject.transform;
            obj.name = mapping.prefab.name + " " + (hierarchyObjects.Find(x => x.path == mapping.name + "s").referenceObject.transform.childCount - 1);
        }

        foreach (var item in patternCheck)
        {
            visittedPixels.Add(item.Item1 * levelMap.height + item.Item2);
        }

        return true;
    }

    private bool CheckWall(ColorMapping mapping, int x, int y)
    {
        (int, int) end_pos = (x, y);

        while (levelMap.GetPixel(x, end_pos.Item2).Equals(mapping.color) && end_pos.Item2 < levelMap.height && !visittedPixels.Contains(x * levelMap.height + end_pos.Item2))
        {
            end_pos.Item2++;
        }

        if (end_pos.Item2 == y + 1)
        {
            return false;
        }

        if (hierarchyObjects.Find(x => x.path == "Walls") == null)
        {
            GameObject g = new("Walls");
            g.transform.parent = hierarchyObjects.Find(x => x.path == "Room").referenceObject.transform;
            g.transform.position = new(0, 0);
            hierarchyObjects.Add(new("Walls", hierarchyObjects.Find(x => x.path == "Room").referenceObject.transform.Find("Walls").gameObject));
        }

        Vector3 position = new(x, ((end_pos.Item2 - y - 1f) / 2f) + y, mapping.zPos);
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(mapping.prefab);
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.parent = hierarchyObjects.Find(x => x.path == "Walls").referenceObject.transform;
        obj.name = mapping.prefab.name + " " + (hierarchyObjects.Find(x => x.path == "Walls").referenceObject.transform.childCount - 1);
        obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y * (end_pos.Item2 - y), obj.transform.localScale.z);

        for (int i = 0; i < end_pos.Item2 - y; i++)
        {
            visittedPixels.Add(x * levelMap.height + y + i);
        }

        return true;
    }

    private bool CheckGround(ColorMapping mapping, int x, int y)
    {
        (int, int) end_pos = (x, y);

        while (levelMap.GetPixel(end_pos.Item1, y).Equals(mapping.color) && end_pos.Item1 < levelMap.width)
        {
            end_pos.Item1++;
        }

        if (hierarchyObjects.Find(x => x.path == "Grounds") == null)
        {
            GameObject g = new("Grounds");
            g.transform.parent = hierarchyObjects.Find(x => x.path == "Room").referenceObject.transform;
            g.transform.position = new(0, 0);
            hierarchyObjects.Add(new("Grounds", hierarchyObjects.Find(x => x.path == "Room").referenceObject.transform.Find("Grounds").gameObject));
        }

        Vector3 position = new(((end_pos.Item1 - x - 1f) / 2f) + x, y, mapping.zPos);
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(mapping.prefab);
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.parent = hierarchyObjects.Find(x => x.path == "Grounds").referenceObject.transform;
        obj.name = mapping.prefab.name + " " + (hierarchyObjects.Find(x => x.path == "Grounds").referenceObject.transform.childCount - 1);
        obj.transform.localScale = new Vector3(obj.transform.localScale.x * (end_pos.Item1 - x), obj.transform.localScale.y, obj.transform.localScale.z);

        for (int i = 0; i < end_pos.Item1 - x; i++)
        {
            visittedPixels.Add((x + i) * levelMap.height + y);
        }

        return true;
    }

    private bool CheckWallGround(ColorMapping wall, ColorMapping ground, int x, int y)
    {
        (int, int) wall_start = (x, y);
        (int, int) ground_start = (x, y);

        if (!levelMap.GetPixel(wall_start.Item1, wall_start.Item2 + 1).Equals(wall.color) || visittedPixels.Contains(wall_start.Item1 * levelMap.height + wall_start.Item2 + 1))
        {
            return false;
        }

        if (levelMap.GetPixel(ground_start.Item1 + 1, ground_start.Item2).Equals(wall.color) && ground_start.Item1 + 1 < levelMap.width)
        {
            bool v = CheckGround(ground, ground_start.Item1, ground_start.Item2);

            if (v)
            {
                wall_start = (x, y + 1);
            }
        }

        (int, int) wall_end = (wall_start.Item1, wall_start.Item2);

        while (levelMap.GetPixel(wall_start.Item1, wall_end.Item2).Equals(wall.color) && wall_end.Item2 < levelMap.height && !visittedPixels.Contains(wall_start.Item1 * levelMap.height + wall_end.Item2))
        {
            wall_end.Item2++;
        }

        if (levelMap.GetPixel(wall_end.Item1 + 1, wall_end.Item2 - 1).Equals(wall.color) && wall_end.Item1 + 1 < levelMap.width)
        {
            ground_start = (wall_end.Item1, wall_end.Item2 - 1);
            bool v = CheckGround(ground, ground_start.Item1, ground_start.Item2);

            if (v)
            {
                wall_end = (wall_end.Item1, wall_end.Item2 - 1);
            }
        }

        if (hierarchyObjects.Find(x => x.path == "Walls") == null)
        {
            GameObject g = new("Walls");
            g.transform.parent = hierarchyObjects.Find(x => x.path == "Room").referenceObject.transform;
            g.transform.position = new(0, 0);
            hierarchyObjects.Add(new("Walls", hierarchyObjects.Find(x => x.path == "Room").referenceObject.transform.Find("Walls").gameObject));
        }

        Vector3 position = new(wall_start.Item1, ((wall_end.Item2 - wall_start.Item2 - 1f) / 2f) + wall_start.Item2, wall.zPos);
        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(wall.prefab);
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.parent = hierarchyObjects.Find(x => x.path == "Walls").referenceObject.transform;
        obj.name = wall.prefab.name + " " + (hierarchyObjects.Find(x => x.path == "Walls").referenceObject.transform.childCount - 1);
        obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y * (wall_end.Item2 - wall_start.Item2), obj.transform.localScale.z);

        for (int i = 0; i < wall_end.Item2 - wall_start.Item2; i++)
        {
            visittedPixels.Add(wall_start.Item1 * levelMap.height + wall_start.Item2 + i);
        }

        return true;
    }

    private bool AngelWall(ColorMapping mapping, int x, int y)
    {
        bool v = CheckAngelWallUp(mapping, x, y) || CheckAngelWallDown(mapping, x, y);

        return v;
    }

    private bool CheckAngelWallUp(ColorMapping mapping, int x, int y)
    {
        (int, int) end_pos = (x, y);

        while (levelMap.GetPixel(end_pos.Item1, end_pos.Item2).Equals(mapping.color) && end_pos.Item1 < levelMap.width && end_pos.Item2 < levelMap.height)
        {
            end_pos.Item1++;
            end_pos.Item2++;
        }

        if (levelMap.GetPixel(end_pos.Item1, end_pos.Item2 - 1).Equals(mapping.color) || levelMap.GetPixel(end_pos.Item1 - 2, end_pos.Item2 - 1).Equals(mapping.color) ||
            levelMap.GetPixel(end_pos.Item1 - 1, end_pos.Item2).Equals(mapping.color) || levelMap.GetPixel(end_pos.Item1 - 1, end_pos.Item2 - 2).Equals(mapping.color))
        {
            end_pos.Item1--;
            end_pos.Item2--;
        }

        if (end_pos.Item1 < x + 2 && end_pos.Item2 < y + 2)
        {
            return false;
        }

        if ((end_pos.Item1 == x + 2 && end_pos.Item2 == y + 2) &&
            (levelMap.GetPixel(end_pos.Item1 - 1, end_pos.Item2 - 2).Equals(mapping.color) || levelMap.GetPixel(end_pos.Item1 - 2, end_pos.Item2 - 1).Equals(mapping.color)))
        {
            return false;
        }

        float scale = Mathf.Sqrt(2f) / 2f;

        // Diffrence between end position x and start position x is equal to diffrence betwteen end position y and start position y
        float scale_factor = Mathf.Sqrt((end_pos.Item1 - x) * (end_pos.Item1 - x) * 2);

        Color a = new(1, 1, 1, 0);

        bool extra_top = !levelMap.GetPixel(end_pos.Item1, end_pos.Item2).a.Equals(a.a);

        bool extra_bottom = !levelMap.GetPixel(x - 1, y - 1).a.Equals(a.a);

        // scale is added to make an object appear to be part of a whole with other objects
        if (extra_top)
        {
            scale_factor += scale;
        }

        if (extra_bottom)
        {
            scale_factor += scale;
        }

        Vector3 position;

        if (extra_top && extra_bottom)
        {
            float shift = 0.5f - scale / 2f;
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x + shift, ((end_pos.Item2 - y - 1f) / 2f) + y + shift, mapping.zPos);
        }
        else if (extra_top && !extra_bottom)
        {
            float shift = (scale / 2f) - 0.25f;
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x + shift, ((end_pos.Item2 - y - 1f) / 2f) + y - 0.5f + shift, mapping.zPos);
        }
        else if (!extra_top && extra_bottom)
        {
            float shift = (scale / 2f) - 0.25f;
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x - shift, ((end_pos.Item2 - y - 1f) / 2f) + y - 0.5f + shift, mapping.zPos);
        }
        else
        {
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x, ((end_pos.Item2 - y - 1f) / 2f) + y, mapping.zPos);
        }


        if (hierarchyObjects.Find(x => x.path == "Walls") == null)
        {
            GameObject g = new("Walls");
            g.transform.parent = hierarchyObjects.Find(x => x.path == "Room").referenceObject.transform;
            g.transform.position = new(0, 0);
            hierarchyObjects.Add(new("Walls", hierarchyObjects.Find(x => x.path == "Room").referenceObject.transform.Find("Walls").gameObject));
        }

        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(mapping.prefab);
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.parent = hierarchyObjects.Find(x => x.path == "Walls").referenceObject.transform;
        obj.name = mapping.prefab.name + " " + (hierarchyObjects.Find(x => x.path == "Walls").referenceObject.transform.childCount - 1);
        obj.transform.localScale = new Vector3(obj.transform.localScale.x * scale_factor, obj.transform.localScale.y, obj.transform.localScale.z);
        obj.transform.Rotate(0f, 0f, 45f);

        for (int i = 0; i < end_pos.Item2 - y; i++)
        {
            visittedPixels.Add((x + i) * levelMap.height + y + i);
        }

        return true;
    }

    private bool CheckAngelWallDown(ColorMapping mapping, int x, int y)
    {
        (int, int) end_pos = (x, y);

        while (levelMap.GetPixel(end_pos.Item1, end_pos.Item2).Equals(mapping.color) && end_pos.Item1 < levelMap.width && end_pos.Item2 >= 0)
        {
            end_pos.Item1++;
            end_pos.Item2--;
        }

        if (levelMap.GetPixel(end_pos.Item1, end_pos.Item2 + 1).Equals(mapping.color) || levelMap.GetPixel(end_pos.Item1 - 2, end_pos.Item2 + 1).Equals(mapping.color) ||
            levelMap.GetPixel(end_pos.Item1 - 1, end_pos.Item2).Equals(mapping.color) || levelMap.GetPixel(end_pos.Item1 - 1, end_pos.Item2 + 2).Equals(mapping.color))
        {
            end_pos.Item1--;
            end_pos.Item2++;
        }

        if (end_pos.Item1 < x + 2 && end_pos.Item2 > y - 2)
        {
            return false;
        }

        if ((end_pos.Item1 == x + 2 && end_pos.Item2 == y - 2) &&
            (levelMap.GetPixel(end_pos.Item1 - 1, end_pos.Item2 + 2).Equals(mapping.color) || levelMap.GetPixel(end_pos.Item1 - 2, end_pos.Item2 + 1).Equals(mapping.color)))
        {
            return false;
        }

        float scale = Mathf.Sqrt(2f) / 2f;

        // Diffrence between end position x and start position x is equal to diffrence betwteen end position y and start position y
        float scale_factor = Mathf.Sqrt((end_pos.Item1 - x) * (end_pos.Item1 - x) * 2);

        Color a = new(1, 1, 1, 0);

        /*bool extra_top = !map.GetPixel(x - 1, y + 1).a.Equals(a.a) || !map.GetPixel(x - 2, y + 1).a.Equals(a.a) || !map.GetPixel(x, y + 1).a.Equals(a.a) ||
            !map.GetPixel(x - 1, y + 2).a.Equals(a.a) || !map.GetPixel(x - 1, y).a.Equals(a.a);

        bool extra_bottom = !map.GetPixel(end_pos.Item1, end_pos.Item2).a.Equals(a.a) || !map.GetPixel(end_pos.Item1 - 1, end_pos.Item2).a.Equals(a.a) ||
            !map.GetPixel(end_pos.Item1, end_pos.Item2 - 1).a.Equals(a.a) || !map.GetPixel(end_pos.Item1, end_pos.Item2 + 1).a.Equals(a.a) ||
            !map.GetPixel(end_pos.Item1 + 1, end_pos.Item2).a.Equals(a.a);*/

        bool extra_top = !levelMap.GetPixel(x - 1, y + 1).a.Equals(a.a);

        bool extra_bottom = !levelMap.GetPixel(end_pos.Item1, end_pos.Item2).a.Equals(a.a);

        // scale is added to make an object appear to be part of a whole with other objects
        if (extra_top)
        {
            scale_factor += scale;
        }

        if (extra_bottom)
        {
            scale_factor += scale;
        }

        Vector3 position;

        if (extra_top && extra_bottom)
        {
            float shift = 0.5f - scale / 2f;
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x + shift, y - ((y - end_pos.Item2 - 1f) / 2f) + shift, mapping.zPos);
        }
        else if (extra_top && !extra_bottom)
        {
            float shift = (scale / 2f) - 0.25f;
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x - shift, y - ((y - end_pos.Item2 - 1f) / 2f) + 0.5f - shift, mapping.zPos);
        }
        else if (!extra_top && extra_bottom)
        {
            float shift = (scale / 2f) - 0.25f;
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x + shift, y - ((y - end_pos.Item2 - 1f) / 2f) - 0.5f + shift, mapping.zPos);
        }
        else
        {
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x, y - ((y - end_pos.Item2 - 1f) / 2f), mapping.zPos);
        }


        if (hierarchyObjects.Find(x => x.path == "Walls") == null)
        {
            GameObject g = new("Walls");
            g.transform.parent = hierarchyObjects.Find(x => x.path == "Room").referenceObject.transform;
            g.transform.position = new(0, 0);
            hierarchyObjects.Add(new("Walls", hierarchyObjects.Find(x => x.path == "Room").referenceObject.transform.Find("Walls").gameObject));
        }

        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(mapping.prefab);
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.parent = hierarchyObjects.Find(x => x.path == "Walls").referenceObject.transform;
        obj.name = mapping.prefab.name + " " + (hierarchyObjects.Find(x => x.path == "Walls").referenceObject.transform.childCount - 1);
        obj.transform.localScale = new Vector3(obj.transform.localScale.x * scale_factor, obj.transform.localScale.y, obj.transform.localScale.z);
        obj.transform.Rotate(0f, 0f, -45f);

        for (int i = 0; i < end_pos.Item1 - x; i++)
        {
            visittedPixels.Add((x + i) * levelMap.height + y - i);
        }

        return true;
    }

    private void MoveEverything(Vector3 diff)
    {
        List<GameObject> elementsToMove = new();

        // Grounds, Patterns, Walls
        Transform tran = hierarchyObjects.Find(x => x.path == "Room").referenceObject.transform;
        for (int i = 0; i < tran.childCount; ++i)
        {
            Transform t = tran.GetChild(i);
            for (int z = 0; z < t.childCount; ++z)
            {
                elementsToMove.Add(t.GetChild(z).gameObject);
            }
        }

        if (hierarchyObjects.Find(x => x.path == "MeshVertex") != null)
        {
            // Vertexes
            tran = hierarchyObjects.Find(x => x.path == "MeshVertex").referenceObject.transform;
            for (int i = 0; i < tran.childCount; ++i)
            {
                elementsToMove.Add(tran.GetChild(i).gameObject);
            }
        }

        if (hierarchyObjects.Find(x => x.path == "OutsideTexture") != null)
        {
            // OutsideTexture
            elementsToMove.Add(hierarchyObjects.Find(x => x.path == "OutsideTexture").referenceObject);
        }

        HierarchyObject p = hierarchyObjects.Find(x => x.path == "Player");

        if (p != null)
        { 
            p.referenceObject.transform.position -= diff;
        }

        foreach (GameObject elem in elementsToMove)
        {
            elem.transform.position -= diff;
        }
    }

    private void MoveEverythingBasedOnPlayer()
    {
        Vector3 playerPos = hierarchyObjects.Find(x => x.path == "Player").referenceObject.transform.position;
        Vector3 diff = new(playerPos.x, playerPos.y, 0f);
        MoveEverything(diff);
    }

    private void MoveEverythingBasedOnMiddlePoint(Vector2 middle)
    {
        Vector3 diff = new(middle.x, middle.y, 0f);
        MoveEverything(diff);
    }

    private void AddCamera()
    {
        GameObject cam = GameObject.Find("Main Camera") ?? new("Main Camera");

        cam.tag = "MainCamera";
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -10.0f);
        
        // Culling Mask

        if (cam.GetComponent<AudioListener>() == null)
        {
            cam.AddComponent<AudioListener>();
        }

        if (cam.GetComponent<CameraFolow>() == null)
        {
            cam.AddComponent<CameraFolow>();
        }

        if (cam.GetComponent<Camera>() == null)
        {
            cam.AddComponent<Camera>();
        }

        Camera c = cam.GetComponent<Camera>();
        c.clearFlags = CameraClearFlags.Skybox;
        c.backgroundColor = _data.cameraBackground;
        c.cullingMask = -1;
        c.orthographic = true;
        c.orthographicSize = _data.cameraSize;
        c.nearClipPlane = 0.3f;
        c.farClipPlane = 1000;
        c.rect = new(0, 0, 1, 1);
        c.depth = -1;

        CameraFolow f = cam.GetComponent<CameraFolow>();
        f.cam = c;
        f.offset = new(0, 0, 0);
        f.smoothFactor = _data.cameraSmoothFactor;

        if (_data.playerMapping.prefab != null)
        {
            f.target = GameObject.Find(_data.playerMapping.name)?.transform;
        }
    }

    private void AddGameTimer()
    {
        GameObject g;

        if ((g = GameObject.Find("GameGlobalTimer")) != null)
        {
            if (g.GetComponent<GameTimer>() != null)
            {
                return;
            }
        }
        else
        {
            g = new("GameGlobalTimer");
        }

        g.AddComponent<GameTimer>();
    }

    private void AddAdditionalObjects()
    {
        foreach (GameObject item in _data.additionalObjects)
        {
            if (item == null)
                continue;

            GameObject g = (GameObject)PrefabUtility.InstantiatePrefab(item);
            g.name = item.name;
        }
    }
}
