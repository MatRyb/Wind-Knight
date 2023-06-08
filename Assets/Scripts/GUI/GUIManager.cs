using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GUIManager : MonoBehaviour
{
    private static GUIManager instance = null;

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject textObject;
    [SerializeField] private GameObject deathScreenObject;
    [SerializeField] private GameObject winScreenObject;
    [SerializeField] private GameObject pauseScreenObject;
    [SerializeField] private GameObject restartQuestionScreenObject;

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

    public static GameObject ShowDeathScreen()
    {
        return instance.ShowDeathScreenJob();
    }

    private GameObject ShowDeathScreenJob()
    {
        return Instantiate(deathScreenObject, canvas.transform, false);
    }

    public static GameObject ShowWinScreen()
    {
        return instance.ShowWinScreenJob();
    }

    private GameObject ShowWinScreenJob()
    {
        return Instantiate(winScreenObject, canvas.transform, false);
    }
    public static GameObject ShowPauseScreen()
    {
        return instance.ShowPauseScreenJob();
    }

    private GameObject ShowPauseScreenJob()
    {
        return Instantiate(pauseScreenObject, canvas.transform, false);
    }

    public static GameObject ShowRestartQuestionScreen()
    {
        return instance.ShowRestartQuestionScreenJob();
    }

    private GameObject ShowRestartQuestionScreenJob()
    {
        return Instantiate(restartQuestionScreenObject, canvas.transform, false);
    }

    public static void HidePauseScreen()
    {
        instance.HidePauseScreenJob();
    }

    private void HidePauseScreenJob()
    {
        Destroy(GameObject.Find("PauseScreen(Clone)"));
    }

    public static void HideRestartQuestionScreen()
    {
        instance.HideRestartQuestionScreenJob();
    }

    private void HideRestartQuestionScreenJob()
    {
        Destroy(GameObject.Find("RestartQuestionScreen(Clone)"));
    }
}
