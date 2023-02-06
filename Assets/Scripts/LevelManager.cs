using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    private static LevelManager instance = null;

    private bool waitingForStart = false;
    private GameObject startText = null;

    private bool waitForRespawn = false;

    private bool waitAfterWin = false;

    private bool paused = false;

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

    // Start is called before the first frame update
    void Start()
    {
        PauseGame(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        waitingForStart = true;
        startText = GUIManager.ShowText("Press 'Left Mouse Button' to start");
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingForStart && Input.GetMouseButtonDown(0))
        {
            StartGame();
        }

        if (waitingForStart)
            return;

        if (waitForRespawn)
            return;

        if (waitAfterWin)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            if (paused)
            {
                ResumeGameJob(false);
            }
            else
            {
                PauseGameJob(false);
            }
        }

        if (Input.GetKeyDown("escape") && !paused)
        {
            PauseGameJob(true);
        }
    }

    public static void InitRespawn()
    {
        GameTimer.StopTime();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameObject screen = GUIManager.ShowDeathScreen();
        Button[] buttons = screen.GetComponentsInChildren<Button>();

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].name == "RespawnBtn")
            {
                buttons[i].onClick.AddListener(() => instance.RespawnJob());
            }
            else if (buttons[i].name == "ExitBtn")
            {
                buttons[i].onClick.AddListener(() => Application.Quit());
            }
        }
        instance.waitForRespawn = true;
    }

    private void RespawnJob()
    {
        GameTimer.StartTime();
        waitForRespawn = false;
        instance = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void InitWinGame()
    {
        GameTimer.StopTime();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameObject screen = GUIManager.ShowWinScreen();
        Button[] buttons = screen.GetComponentsInChildren<Button>();

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].name == "RestartBtn")
            {
                buttons[i].onClick.AddListener(() => instance.RespawnJob());
            }
            else if (buttons[i].name == "ExitBtn")
            {
                buttons[i].onClick.AddListener(() => Application.Quit());
            }
        }
        instance.waitAfterWin = true;
    }

    private void StartGame()
    {
        Destroy(startText);
        waitingForStart = false;
        ResumeGame(false);
    }

    public static void PauseGame(bool isScreen)
    {
        instance.PauseGameJob(isScreen);
    }

    private void PauseGameJob(bool isScreen)
    {
        GameTimer.StopTime();
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (isScreen)
        {
            GameObject screen = GUIManager.ShowPauseScreen();
            Button[] buttons = screen.GetComponentsInChildren<Button>();

            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].name == "RestartBtn")
                {
                    buttons[i].onClick.AddListener(() => instance.RespawnJob());
                }
                else if (buttons[i].name == "ResumeBtn")
                {
                    buttons[i].onClick.AddListener(() => instance.ResumeGameJob(isScreen));
                }
                else if (buttons[i].name == "ExitBtn")
                {
                    buttons[i].onClick.AddListener(() => Application.Quit());
                }
            }
        }
    }

    public static void ResumeGame(bool isScreen)
    {
        instance.ResumeGameJob(isScreen);
    }

    private void ResumeGameJob(bool isScreen)
    {
        GameTimer.StartTime();
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (isScreen)
        {
            GUIManager.HidePauseScreen();
        }
    }
}
