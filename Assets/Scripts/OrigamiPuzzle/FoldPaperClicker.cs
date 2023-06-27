using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class FoldPaperClicker : MonoBehaviour
{
    public OrigamiPuzzleManager manager;
    public int foldIndex;
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

    public GameObject orderCanvas;
    public TextMeshProUGUI orderText;

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
            manager.HighlightFoldLine(foldIndex, true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (OrderIndex == -1)
        {
            manager.HighlightFoldLine(foldIndex, true);
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (OrderIndex != -1)
            {
                manager.HighlightFoldLine(foldIndex, true);
            }
            else
            {
                manager.HighlightFoldLine(foldIndex, false);
            }
            manager.FoldButtonClicked(foldIndex);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (OrderIndex == -1)
        {
            manager.HighlightFoldLine(foldIndex, false);
        }
    }
}
