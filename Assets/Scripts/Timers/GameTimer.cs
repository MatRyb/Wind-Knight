using UnityEngine;
using NaughtyAttributes;

public class GameTimer : MonoBehaviour
{
    public static float timeMultiplayer { get; private set; } = 1f;

    [Button]
    public static void StopTime()
    {
        timeMultiplayer = 0f;
    }

    [Button]
    public static void StartTime()
    {
        timeMultiplayer = 1f;
    }
}
