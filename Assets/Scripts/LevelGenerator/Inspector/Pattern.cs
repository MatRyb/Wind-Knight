using UnityEngine;

[System.Serializable]
public class Pattern
{
    public bool ul;
    public bool um;
    public bool ur;
    public bool ml;
    public bool mm;
    public bool mr;
    public bool dl;
    public bool dm;
    public bool dr;

    public void Set(int index, bool b)
    {
        if (index < 0)
        {
            Debug.LogError("LevelGenerator (Pattern): Index out of bounds!!");
            return;
        }

        switch (index)
        {
            case 0:
                dl = b;
                break;
            case 1:
                ml = b;
                break;
            case 2:
                ul = b;
                break;
            case 3:
                dm = b;
                break;
            case 4:
                mm = b;
                break;
            case 5:
                um = b;
                break;
            case 6:
                dr = b;
                break;
            case 7:
                mr = b;
                break;
            case 8:
                ur = b;
                break;
            default:
                break;
        }
    }

    public bool Get(int index)
    {
        if (index < 0)
        {
            Debug.LogError("LevelGenerator (Pattern): Index out of bounds!!");
            return false;
        }

        switch (index)
        {
            case 0:
                return dl;
            case 1:
                return ml;
            case 2:
                return ul;
            case 3:
                return dm;
            case 4:
                return mm;
            case 5:
                return um;
            case 6:
                return dr;
            case 7:
                return mr;
            case 8:
                return ur;
            default:
                return false;
        }
    }
}
