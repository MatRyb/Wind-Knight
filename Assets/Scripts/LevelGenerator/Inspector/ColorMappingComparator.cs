using UnityEngine;
using System.Collections.Generic;

public class ColorMappingComparator : IComparer<ColorMapping>
{
    public int Compare(ColorMapping x, ColorMapping y)
    {
        int x_value = int.Parse(ColorUtility.ToHtmlStringRGBA(x.color), System.Globalization.NumberStyles.HexNumber);
        int y_value = int.Parse(ColorUtility.ToHtmlStringRGBA(y.color), System.Globalization.NumberStyles.HexNumber);

        if (x_value < y_value)
        {
            return -1;
        }
        else if (x_value > y_value)
        {
            return 1;
        }
        else
        {
            if (x.type < y.type)
            {
                return -1;
            }
            else if (x.type > y.type)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
