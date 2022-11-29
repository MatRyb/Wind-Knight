using UnityEngine;
using UnityEngine.SceneManagement;

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
        startText = GUIManager.ShowText("Press 'Space' to start", Color.black, Vector2.zero, UIRelativity.middleCenter);
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingForStart && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }

        if (waitingForStart)
            return;

        if (waitForRespawn && Input.GetKeyDown(KeyCode.R))
        {
            RespawnJob();
        }

        if (waitForRespawn)
            return;

        if (waitAfterWin)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RespawnJob();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            return;
        }            

        if (Input.GetKeyDown(KeyCode.Escape))
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
        GUIManager.ShowText("You Died\nPress 'R' to Respawn", Color.black, Vector2.zero, UIRelativity.middleCenter);
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
        GUIManager.ShowText("You Won\nPress 'R' to Restart\nor Press 'Esc' to Exit", Color.black, Vector2.zero, UIRelativity.middleCenter);
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
