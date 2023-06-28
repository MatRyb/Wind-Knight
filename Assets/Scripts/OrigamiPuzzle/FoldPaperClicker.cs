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

    private void Start()
    {
        orderCanvas.SetActive(false);
        spriteRenderer.color = buttonEnableColor;
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
        if (OrderIndex == -1)
        {
            manager.HighlightFoldLine(fold.line, true);
        }
        else
        {
            manager.HighlightFoldLine(fold.line, false);
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (OrderIndex != -1)
            {
                manager.HighlightFoldLine(fold.line, true);
            }
            else
            {
                manager.HighlightFoldLine(fold.line, false);
            }
            manager.FoldButtonClicked(this);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (OrderIndex == -1)
        {
            manager.HighlightFoldLine(fold.line, true);
        }
        else
        {
            manager.HighlightFoldLine(fold.line, false);
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (OrderIndex != -1)
            {
                manager.HighlightFoldLine(fold.line, true);
            }
            else
            {
                manager.HighlightFoldLine(fold.line, false);
            }
            manager.FoldButtonClicked(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (OrderIndex == -1)
        {
            manager.HighlightFoldLine(fold.line, false);
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
