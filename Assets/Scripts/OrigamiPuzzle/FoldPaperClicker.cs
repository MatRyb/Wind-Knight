using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class FoldPaperClicker : MonoBehaviour
{
    // Structs:
    [System.Serializable]
    public struct FoldLine
    {
        [SerializeField] private Vector2 pointA;
        [SerializeField] private Vector2 pointB;

        public Vector3 GetPoint(float t)
        {
            return pointA + (pointB - pointA) * t;
        }

        public static bool operator==(FoldLine a, FoldLine b)
        {
            return a.pointA == b.pointA && a.pointB == b.pointB;
        }

        public static bool operator !=(FoldLine a, FoldLine b)
        {
            return !(a == b);
        }
    }

    [System.Serializable]
    public struct FoldData
    {
        public FoldLine line;
        public float angle;
    }

    public OrigamiPuzzleManager manager;
    public FoldData fold;
    private int orderIndex = -1;
    public int OrderIndex
    {
        get
        {
            return orderIndex;
        }
        set
        {
            orderIndex = value;
            OrderChanged();
        }
    }

    [Header("Canvas:")]
    public GameObject orderCanvas;
    public TextMeshProUGUI orderText;

    [Header("Sprite:")]
    public SpriteRenderer spriteRenderer;
    public Color buttonEnableColor;
    public Color buttonDisableColor;

    private bool playerInTrigger = false;
    private bool PlayerInTrigger 
    { 
        get
        {
            return playerInTrigger;
        }

        set
        {
            if (playerInTrigger != value)
            {
                playerInTrigger = value;
                UpdateHighlight();
            }
        }
    }

    private void Start()
    {
        orderCanvas.SetActive(false);
        spriteRenderer.color = buttonEnableColor;
    }

    private void Update()
    {
        if (PlayerInTrigger)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (OrderIndex != -1)
                {
                    manager.HighlightFoldLine(this, true);
                }
                else
                {
                    manager.HighlightFoldLine(this, false);
                }
                manager.FoldButtonClicked(this);
            }
        }
    }

    private void UpdateHighlight()
    {
        if (PlayerInTrigger)
        {
            if (OrderIndex == -1)
            {
                manager.HighlightFoldLine(this, true);
            }
            else
            {
                manager.HighlightFoldLine(this, false);
            }
        }
        else
        {
            manager.HighlightFoldLine(this, false);
        }
    }

    private void OrderChanged()
    {
        if (OrderIndex != -1)
        {
            orderCanvas.SetActive(true);
            orderText.text = string.Format("{0}", OrderIndex + 1);
            spriteRenderer.color = buttonDisableColor;
        }
        else
        {
            orderCanvas.SetActive(false);
            spriteRenderer.color = buttonEnableColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nie mo¿e byæ: collision.CompareTag("Player")
        if (collision.TryGetComponent<PlayerControler>(out var controler))
        {
            PlayerInTrigger = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerControler>(out var controler))
        {
            PlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerControler>(out var controler))
        {
            PlayerInTrigger = false;
        }
    }

    public void OnDrawGizmosSelected()
    {
        var line = fold.line;
        Gizmos.color = Color.red;
        Vector3 pointA = manager.PercentPointToGlobalPoint(line.GetPoint(0f));
        Vector3 midPoint = manager.PercentPointToGlobalPoint(line.GetPoint(.5f));
        Vector3 pointB = manager.PercentPointToGlobalPoint(line.GetPoint(1f));
        Gizmos.DrawSphere(pointA, .4f);
        Gizmos.DrawSphere(pointB, .4f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pointA, pointB);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(midPoint, .3f);
    }
}
