using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalTimer
{
    private bool paused = true;

    public float timeToEnd { get; private set; }
    public Queue<Action> actions = new Queue<Action>();

    public LocalTimer(float totalTime)
    {
        timeToEnd = totalTime;
    }

    public LocalTimer Start()
    {
        paused = false;
        return this;
    }

    public LocalTimer Pause()
    {
        paused = true;
        return this;
    }

    public void UpdateTime()
    {
        if (paused)
            return;

        timeToEnd -= Time.deltaTime * GameTimer.timeMultiplayer;
        if (timeToEnd <= 0f)
        {
            OnEnd();
        }
    }

    private void OnEnd()
    {
        for (int i = 0; i < actions.Count; ++i)
        {
            Action a = actions.Dequeue();
            a.Invoke();
        }
    }

    public LocalTimer DoAfter(Action action)
    {
        actions.Enqueue(action);
        return this;
    }
}
