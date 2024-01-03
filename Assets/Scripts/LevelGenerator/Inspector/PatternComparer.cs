using System.Collections.Generic;

class PatternComparer : IEqualityComparer<Pattern>
{
    // Patterns are equal if their nine bool values are equal.
    public bool Equals(Pattern x, Pattern y)
    {

        //Check whether the compared objects reference the same data.
        if (ReferenceEquals(x, y)) return true;

        //Check whether any of the compared objects is null.
        if (x is null || y is null)
            return false;

        //Check whether the patterns' properties are equal.
        return x.ul == y.ul && x.um == y.um && x.ur == y.ur && x.ml == y.ml && x.mm == y.mm && x.mr == y.mr && x.dl == y.dl && x.dm == y.dm && x.dr == y.dr;
    }

    // If Equals() returns true for a pair of objects
    // then GetHashCode() must return the same value for these objects.

    public int GetHashCode(Pattern pattern)
    {
        //Check whether the object is null
        if (pattern is null) return 0;

        //Calculate the hash code for the pattern.
        return pattern.ul.GetHashCode() ^ pattern.um.GetHashCode() ^ pattern.ur.GetHashCode() ^ pattern.ml.GetHashCode() ^ 
            pattern.mm.GetHashCode() ^ pattern.mr.GetHashCode() ^ pattern.dl.GetHashCode() ^ pattern.dm.GetHashCode() ^ pattern.dr.GetHashCode();
    }
}
