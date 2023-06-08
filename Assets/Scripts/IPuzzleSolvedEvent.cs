using UnityEngine;

[System.Serializable]
public abstract class IPuzzleSolvedEvent : MonoBehaviour
{
    public abstract void Solved();
    public abstract void Unsolved();
}
