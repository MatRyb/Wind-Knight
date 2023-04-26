using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchColorPuzzle : MonoBehaviour
{
    [HideInInspector] public bool IsInTrigger = false;
    private int puzzleLayer;

    private void Start()
    {
        puzzleLayer = LayerMask.NameToLayer("Puzzle");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer==puzzleLayer)
        {
            IsInTrigger = true;
            Debug.Log("Object entered the puzzle trigger");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == puzzleLayer)
        {
            IsInTrigger = false;
            Debug.Log("Object left the puzzle trigger");
        }
    }
}
