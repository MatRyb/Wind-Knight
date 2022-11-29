using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum UIRelativity { topLeft, topCenter, topRight, middleLeft, middleCenter, middleRight, bottomLeft, bottomCenter, bottomRight };
public class GUIManager : MonoBehaviour
{
    private static GUIManager instance = null;

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject infoObject;

    private void OnEnable()
    {
        if (instance != this && instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Awake()
    {
        if (instance != this && instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public static GameObject ShowText(string text, Color color, Vector2 position, UIRelativity relativity, Transform parent = null)
    {
        Vector2 anchorMin;
        Vector2 anchorMax;
        TranslateAnchores(relativity, out anchorMin, out anchorMax);
        return instance.ShowTextJob(text, color, position, anchorMin, anchorMax, parent);
    }

    private GameObject ShowTextJob(string text, Color color, Vector2 position, Vector2 anchorMin, Vector2 anchorMax, Transform parent)
    {
        GameObject gameObject = new GameObject("showText");
        if (parent == null)
        {
            gameObject.transform.parent = canvas.transform;
        }
        else
        {
            gameObject.transform.parent = parent;
        }

        TextMeshProUGUI textUI = gameObject.AddComponent<TextMeshProUGUI>();
        textUI.autoSizeTextContainer = true;
        textUI.text = text;
        textUI.color = color;
        textUI.rectTransform.anchorMin = anchorMin;
        textUI.rectTransform.anchorMax = anchorMax;
        textUI.rectTransform.anchoredPosition = position;

        return gameObject;
    }

    public static GameObject ShowInfo(string text)
    {
        return instance.ShowInfoJob(text);
    }

    private GameObject ShowInfoJob(string text)
    {
        GameObject gameObject = Instantiate(infoObject, canvas.transform, false);
        gameObject.GetComponent<InfoGUIObject>().SetInfoText(text);
        return gameObject;
    }

    public static void TranslateAnchores(UIRelativity relativity, out Vector2 anchorMin, out Vector2 anchorMax)
    {
        anchorMin = new Vector2(.5f, .5f);
        anchorMax = new Vector2(.5f, .5f);
        switch (relativity)
        {
            case UIRelativity.topLeft:
                anchorMin = new Vector2(0f, 1f);
                anchorMax = new Vector2(0f, 1f);
                break;
            case UIRelativity.topCenter:
                anchorMin = new Vector2(0.5f, 1f);
                anchorMax = new Vector2(0.5f, 1f);
                break;
            case UIRelativity.topRight:
                anchorMin = new Vector2(1f, 1f);
                anchorMax = new Vector2(1f, 1f);
                break;
            case UIRelativity.middleLeft:
                anchorMin = new Vector2(0f, .5f);
                anchorMax = new Vector2(0f, .5f);
                break;
            case UIRelativity.middleRight:
                anchorMin = new Vector2(1f, .5f);
                anchorMax = new Vector2(1f, .5f);
                break;
            case UIRelativity.bottomLeft:
                anchorMin = new Vector2(0f, 0f);
                anchorMax = new Vector2(0f, 0f);
                break;
            case UIRelativity.bottomCenter:
                anchorMin = new Vector2(.5f, 0f);
                anchorMax = new Vector2(.5f, 0f);
                break;
            case UIRelativity.bottomRight:
                anchorMin = new Vector2(1f, 0f);
                anchorMax = new Vector2(1f, 0f);
                break;
            case UIRelativity.middleCenter:
            default:
                break;
        }
    }
}
