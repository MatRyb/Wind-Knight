using UnityEngine;
using System.Collections.Generic;

class ColorMappingComparer : IEqualityComparer<ColorMapping>
{
    // Color Mappings are equal if their color and type are equal. Unless it is pattern then pattern is checked.
    public bool Equals(ColorMapping x, ColorMapping y)
    {
        //Check whether the compared objects reference the same data.
        if (ReferenceEquals(x, y)) return true;

        //Check whether any of the compared objects is null.
        if (x is null || y is null)
            return false;

        //Check whether the Color Mappings' properties are equal.
        return x.type != MappingType.Pattern ? x.color == y.color && x.type == y.type : x.color == y.color && x.type == y.type && x.pattern.Equals(y.pattern);
    }

    // If Equals() returns true for a pair of objects
    // then GetHashCode() must return the same value for these objects.

    public int GetHashCode(ColorMapping mapping)
    {
        //Check whether the object is null
        if (mapping is null) return 0;

        //Calculate the hash code for the Color Mapping.
        return mapping.type != MappingType.Pattern ? mapping.color.GetHashCode() ^ mapping.type.GetHashCode() :
            mapping.color.GetHashCode() ^ mapping.type.GetHashCode() ^ mapping.pattern.GetHashCode();
    }
}