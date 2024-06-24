using UnityEngine;

public class AndPuzzleSolver : IPuzzleSolvedEvent
{
    [SerializeField] private uint toSolveCount;
    private uint solvedCount = 0;
    [SerializeField] private IPuzzleSolvedEvent puzzleSolvedEvent;

    public override void Solved()
    {
        if (solvedCount == toSolveCount) return;

        ++solvedCount;
        if (solvedCount == toSolveCount)
        {
            puzzleSolvedEvent.Solved();
        }
    }

    public override void Unsolved()
    {
        if (solvedCount == 0) return;
        
        --solvedCount;
        if (solvedCount == toSolveCount - 1)
        {
            puzzleSolvedEvent.Unsolved();
        }
    }
}
