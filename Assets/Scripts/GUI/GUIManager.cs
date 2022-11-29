using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIManager : MonoBehaviour
{
    private static GUIManager instance = null;

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject infoObject;
    [SerializeField] private GameObject textObject;

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

    public static GameObject ShowText(string text)
    {
        return instance.ShowTextJob(text);
    }

    private GameObject ShowTextJob(string text)
    {
        GameObject gameObject = Instantiate(textObject, canvas.transform, false);
        gameObject.GetComponent<TextMeshProUGUI>().text = text;
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
}
