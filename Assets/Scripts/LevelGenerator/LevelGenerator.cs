using UnityEngine;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// 
/// READING IMAGE GOES FROM DOWN LEFT TO UPPER RIGHT!!!
/// 
/// 
/// TODO:
/// - Add deleting repeated wall and ground color definition in sorted list
/// - Add decoding of vertex map
/// - Add generating planes based on vertex map by heritating of created scripts
/// - Add generating big plane behind when generating map (with size of map + 20)
/// - Create new empty object (Room) where will be stored grouped objects by type (if Pattern -> by name)
/// - Add Room counter
/// - Add deleting choosen rooms
/// - Create Editor
/// - Add Objects
/// - Add Enemies
/// - Add Checkpoints
/// - Player spawn
/// </summary>


public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Texture2D levelMap;
    [SerializeField] private Texture2D vertexMap;

    [HelpBox("Checking which object type will be used for a given pixel color is in the order: \n Pattern > Wall (Including Angel Wall) > Ground", HelpBoxMessageType.Info)]
    [SerializeField] private ColorMapping[] colorMappings;

    [ReadOnly]
    [SerializeField] private int objectsCount = 0;

    private List<int> visittedPixels;

    public void GenerateLevel()
    {
        if (levelMap == null)
        {
            Debug.LogError("LevelGenerator: Level Map Texture is null. Please provide one.");
            return;
        }

        if (colorMappings.Length == 0)
        {
            Debug.LogError("LevelGenerator: No ColorMappings were defined. Please define one.");
            return;
        }

        visittedPixels = new();

        int allPixels = levelMap.width * levelMap.height;

        List<ColorMapping> sorted = colorMappings.OrderByDescending(c => c, new ColorMappingComparator()).ToList();

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
    }

    public void DeleteLevel()
    {
        if (objectsCount == 0)
        {
            return;
        }

        for (int i = 0; i < transform.childCount; ++i)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
            --objectsCount;
        }

        if (transform.childCount != 0)
        {
            DeleteLevel();
        }
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
                            Instantiate(colorMapping.prefab, position, Quaternion.identity, transform);
                            ++objectsCount;
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

    private bool IsWallDefinied(int index, List<ColorMapping> mappings)
    {
        return index - 1 >= 0 && mappings[index - 1].color.Equals(mappings[index].color) && mappings[index - 1].type == MappingType.Wall;
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

        List<(int, int)> patternCheck = new();

        patternCheck.Add((x, y));

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

        Vector2 position = new(beg_x + 1, beg_y + 1);
        Instantiate(mapping.prefab, position, Quaternion.identity, transform);
        ++objectsCount;

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

        Vector2 position = new(x, ((end_pos.Item2 - y - 1f) / 2f) + y);
        GameObject obj = Instantiate(mapping.prefab, position, Quaternion.identity, transform);
        obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y * (end_pos.Item2 - y), obj.transform.localScale.z);
        ++objectsCount;

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

        Vector2 position = new(((end_pos.Item1 - x - 1f) / 2f) + x, y);
        GameObject obj = Instantiate(mapping.prefab, position, Quaternion.identity, transform);
        obj.transform.localScale = new Vector3(obj.transform.localScale.x * (end_pos.Item1 - x), obj.transform.localScale.y, obj.transform.localScale.z);
        ++objectsCount;

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

        Vector2 position = new(wall_start.Item1, ((wall_end.Item2 - wall_start.Item2 - 1f) / 2f) + wall_start.Item2);
        GameObject obj = Instantiate(wall.prefab, position, Quaternion.identity, transform);
        obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y * (wall_end.Item2 - wall_start.Item2), obj.transform.localScale.z);
        ++objectsCount;

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

        /*bool extra_top = !map.GetPixel(end_pos.Item1, end_pos.Item2).a.Equals(a.a) || !map.GetPixel(end_pos.Item1 - 1, end_pos.Item2).a.Equals(a.a) ||
            !map.GetPixel(end_pos.Item1, end_pos.Item2 - 1).a.Equals(a.a) || !map.GetPixel(end_pos.Item1, end_pos.Item2 + 1).a.Equals(a.a) ||
            !map.GetPixel(end_pos.Item1 + 1, end_pos.Item2).a.Equals(a.a);

        bool extra_bottom = !map.GetPixel(x - 1, y - 1).a.Equals(a.a) || !map.GetPixel(x - 2, y - 1).a.Equals(a.a) || !map.GetPixel(x, y - 1).a.Equals(a.a) ||
            !map.GetPixel(x - 1, y - 2).a.Equals(a.a) || !map.GetPixel(x - 1, y).a.Equals(a.a);*/

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

        Vector2 position;

        if (extra_top && extra_bottom)
        {
            float shift = 0.5f - scale / 2f;
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x + shift, ((end_pos.Item2 - y - 1f) / 2f) + y + shift);
        }
        else if (extra_top && !extra_bottom)
        {
            float shift = (scale / 2f) - 0.25f;
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x + shift, ((end_pos.Item2 - y - 1f) / 2f) + y - 0.5f + shift);
        }
        else if (!extra_top && extra_bottom)
        {
            float shift = (scale / 2f) - 0.25f;
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x - shift, ((end_pos.Item2 - y - 1f) / 2f) + y - 0.5f + shift);
        }
        else
        {
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x, ((end_pos.Item2 - y - 1f) / 2f) + y);
        }

        GameObject obj = Instantiate(mapping.prefab, position, Quaternion.identity, transform);

        obj.transform.localScale = new Vector3(obj.transform.localScale.x * scale_factor, obj.transform.localScale.y, obj.transform.localScale.z);
        obj.transform.Rotate(0f, 0f, 45f);
        ++objectsCount;

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

        Vector2 position;

        if (extra_top && extra_bottom)
        {
            float shift = 0.5f - scale / 2f;
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x + shift, y - ((y - end_pos.Item2 - 1f) / 2f) + shift);
        }
        else if (extra_top && !extra_bottom)
        {
            float shift = (scale / 2f) - 0.25f;
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x - shift, y - ((y - end_pos.Item2 - 1f) / 2f) + 0.5f - shift);
        }
        else if (!extra_top && extra_bottom)
        {
            float shift = (scale / 2f) - 0.25f;
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x + shift, y - ((y - end_pos.Item2 - 1f) / 2f) - 0.5f + shift);
        }
        else
        {
            position = new(((end_pos.Item1 - x - 1f) / 2f) + x, y - ((y - end_pos.Item2 - 1f) / 2f));
        }

        GameObject obj = Instantiate(mapping.prefab, position, Quaternion.identity, transform);

        obj.transform.localScale = new Vector3(obj.transform.localScale.x * scale_factor, obj.transform.localScale.y, obj.transform.localScale.z);
        obj.transform.Rotate(0f, 0f, -45f);
        ++objectsCount;

        for (int i = 0; i < end_pos.Item1 - x; i++)
        {
            visittedPixels.Add((x + i) * levelMap.height + y - i);
        }

        return true;
    }
}
