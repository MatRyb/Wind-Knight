using UnityEngine;
using UnityEngine.SceneManagement;

public enum MenuWindow { Title, LevelSelect, Options, Credits }

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    public MenuWindow menuState;

    [Header("Canvases:")]
    public GameObject TitleScreen;
    public GameObject LevelSelectScreen;
    public GameObject OptionsScreen;
    public GameObject CreditsScreen;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        menuState = MenuWindow.Title;
        TitleScreen.SetActive(true);
        LevelSelectScreen.SetActive(false);
        OptionsScreen.SetActive(false);
        CreditsScreen.SetActive(false);
    }

    public void OpenLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenTitle()
    {
        if (menuState == MenuWindow.Credits)
        {
            CreditsScreen.SetActive(false);
            TitleScreen.SetActive(true);
        }
        else if (menuState == MenuWindow.Options)
        {
            OptionsScreen.SetActive(false);
            TitleScreen.SetActive(true);
        }
        else if (menuState == MenuWindow.LevelSelect)
        {
            LevelSelectScreen.SetActive(false);
            TitleScreen.SetActive(true);
        }
        menuState = MenuWindow.Title;
    }

    public void OpenLevelSelect()
    {
        if (menuState == MenuWindow.Title)
        {
            TitleScreen.SetActive(false);
            LevelSelectScreen.SetActive(true);
        }
        menuState = MenuWindow.LevelSelect;
    }

    public void OpenCredits()
    {
        if (menuState == MenuWindow.Title)
        {
            TitleScreen.SetActive(false);
            CreditsScreen.SetActive(true);
        }
        menuState = MenuWindow.Credits;
    }

    public void OpenOptions()
    {
        if (menuState == MenuWindow.Title)
        {
            TitleScreen.SetActive(false);
            OptionsScreen.SetActive(true);
        }
        menuState = MenuWindow.Options;
    }
}
