using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalTimerContainer
{
    private LocalTimer timer = null;

    public LocalTimerContainer(ref LocalTimer timer)
    {
        this.timer = timer;
    }

    public LocalTimerContainer Start()
    {
        this.timer.Start();
        return this;
    }

    public LocalTimerContainer Pause()
    {
        this.timer.Pause();
        return this;
    }

    public LocalTimerContainer DoAfter(Action action)
    {
        this.timer.DoAfter(action);
        return this;
    }
}

public class LocalTimersManager : MonoBehaviour
{
    public static LocalTimersManager instance = null;
    private List<LocalTimer> timers = new List<LocalTimer>();

    private static LocalTimersManager Init()
    {
        if (instance != null)
        {
            return instance;
        }

        var obj = new GameObject("Local Timers Handler");
        instance = obj.AddComponent<LocalTimersManager>();
        return instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (timers.Count == 0)
        {
            return;
        }

        foreach (var timer in timers)
        {
            timer.UpdateTime();
            if (timer.timeToEnd <= 0f)
            {
                timers.Remove(timer);
                if (timers.Count == 0)
                {
                    break;
                }
            }
        }
    }

    public static LocalTimerContainer CreateNewTimer(float totalTime)
    {
        LocalTimersManager timersHandler = LocalTimersManager.Init();
        LocalTimer newTimer = new LocalTimer(totalTime);
        timersHandler.timers.Add(newTimer);
        return new LocalTimerContainer(ref newTimer);
    }
}
