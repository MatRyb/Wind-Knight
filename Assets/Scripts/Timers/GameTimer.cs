using UnityEngine;
using NaughtyAttributes;

public class GameTimer : MonoBehaviour
{
    public static readonly float STOPPED = 0f;
    public static readonly float PLAYING = 1f;

    public static float TimeMultiplier { get; private set; } = PLAYING;

    [Button]
    public static void StopTime()
    {
        TimeMultiplier = STOPPED;
    }

    [Button]
    public static void StartTime()
    {
        TimeMultiplier = PLAYING;
    }
}
