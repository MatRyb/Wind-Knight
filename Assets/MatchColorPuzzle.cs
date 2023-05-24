using UnityEngine;

public class MatchColorPuzzle : MonoBehaviour
{
    [HideInInspector] public bool IsInTrigger = false;
    private int puzzleLayer;
    [SerializeField] private IPuzzleSolvedEvent puzzleSolvedEvent;

    private void Start()
    {
        puzzleLayer = LayerMask.NameToLayer("Puzzle");
        puzzleSolvedEvent.Unsolved();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer==puzzleLayer)
        {
            IsInTrigger = true;
            puzzleSolvedEvent.Solved();
            Debug.Log("Object entered the puzzle trigger");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == puzzleLayer)
        {
            IsInTrigger = false;
            puzzleSolvedEvent.Unsolved();
            Debug.Log("Object left the puzzle trigger");
        }
    }
}
