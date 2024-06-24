using System.Collections.Generic;
using UnityEngine;

public class ItemsInBagPuzzle : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToBeInBag;
    private readonly List<GameObject> objectsInBag = new();
    [SerializeField] private IPuzzleSolvedEvent puzzleSolvedEvent;

    private void Start()
    {
        puzzleSolvedEvent.Unsolved();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (objectsInBag.Contains(collision.gameObject)) return;

        foreach (var obj in objectsToBeInBag)
        {
            if (collision.gameObject == obj)
            {
                objectsInBag.Add(obj);
                if (objectsInBag.Count == objectsToBeInBag.Length)
                {
                    puzzleSolvedEvent.Solved();
                }
                break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (objectsInBag.Contains(collision.gameObject))
        {
            objectsInBag.Remove(collision.gameObject);
            if (objectsInBag.Count == objectsToBeInBag.Length - 1)
            {
                puzzleSolvedEvent.Unsolved();
            }
        }
    }
}
