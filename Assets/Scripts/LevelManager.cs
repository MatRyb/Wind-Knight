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
        PauseGame();
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
                ResumeGameJob();
            }
            else
            {
                PauseGameJob();
            }
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
        ResumeGame();
    }

    public static void PauseGame()
    {
        instance.PauseGameJob();
    }

    private void PauseGameJob()
    {
        GameTimer.StopTime();
        paused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void ResumeGame()
    {
        instance.ResumeGameJob();
    }

    private void ResumeGameJob()
    {
        GameTimer.StartTime();
        paused = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
