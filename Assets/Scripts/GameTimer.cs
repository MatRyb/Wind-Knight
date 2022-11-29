using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static float timeMultiplayer { get; private set; } = 1f;
    [SerializeField] private float TimeMultiplayer;

    private void Update()
    {
        TimeMultiplayer = timeMultiplayer;
    }

    public static void StopTime()
    {
        timeMultiplayer = 0f;
    }

    public static void StartTime()
    {
        timeMultiplayer = 1f;
    }
}
