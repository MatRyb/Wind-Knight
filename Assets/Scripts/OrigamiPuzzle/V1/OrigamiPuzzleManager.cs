using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OrigamiPuzzleManager : MonoBehaviour
{
    [SerializeField] private GameObject interactInfoText;
    [SerializeField] private OrigamiUIPuzzleManager puzzleUI;

    private void Start()
    {
        interactInfoText.SetActive(false);
    }

    private void Update()
    {
        if (interactInfoText.activeSelf && !puzzleUI.IsUIOn())
        {
            if (Input.GetMouseButtonDown(1))
            {
                puzzleUI.SetUIOn();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactInfoText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactInfoText.SetActive(false);
        }
    }
}
