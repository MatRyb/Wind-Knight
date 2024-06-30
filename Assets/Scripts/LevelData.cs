using UnityEngine;

[CreateAssetMenu(fileName = "newLevelData", menuName = "Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public string Name;
    public float Star3Time;
    public float Star2Time;
    public float Star1Time;

    [HideInInspector] public Color ActiveStar = new(0.99f, 1f, 0.47f);
    [HideInInspector] public Color DeactiveStar = new(0.4f, 0.4f, 0.4f);

    public uint GetStars(float time)
    {
        if (time < 0f) return 0;

        if (time < Star3Time) return 3;
        else if (time < Star2Time) return 2;
        else if (time < Star1Time) return 1;
        else return 0;
    }
}