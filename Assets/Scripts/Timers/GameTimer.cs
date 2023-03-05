using UnityEngine;
using NaughtyAttributes;

public class GameTimer : MonoBehaviour
{
    public static float timeMultiplayer { get; private set; } = 1f;
    [SerializeField] private float TimeMultiplayer;

    private void Update()
    {
        TimeMultiplayer = timeMultiplayer;
    }

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
