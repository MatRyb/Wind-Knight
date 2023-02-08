using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    private class RespawnPoint
    {
        public int id = 0;
        public Vector3 position = new Vector3(0,0,0);

        public RespawnPoint(int _id, Vector3 _pos)
        {
            id = _id;
            position = _pos;
        }
    };

    private static LevelManager instance = null;

    private bool waitingForStart = false;
    private GameObject startText = null;

    private bool waitForRespawn = false;

    private bool waitAfterWin = false;

    private bool paused = false;

    private bool start;

    private RespawnPoint resp = new RespawnPoint(0, new Vector3(0,0,0));

    void OnLevelWasLoaded(int level)
    {
        instance.start = true;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        instance.start = true;
    }

    void Update()
    {
        if (instance.start)
        {
            instance.start = false;
            GameObject player = FindObjectOfType<PlayerControler>().gameObject;
            Camera.main.transform.position = new Vector3(instance.resp.position.x, instance.resp.position.y, Camera.main.transform.position.z);
            player.transform.position = instance.resp.position;
            player.GetComponent<PlayerControler>().mouseInit();
            PauseGame(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            waitingForStart = true;
            startText = GUIManager.ShowText("Press 'Left Mouse Button' to start");
        }

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
            else if (buttons[i].name == "RestartBtn")
            {
                buttons[i].onClick.AddListener(() => instance.RestartJob());
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
        waitForRespawn = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void RestartJob()
    {
        waitForRespawn = false;
        instance.resp.id = 0;
        instance.resp.position = new Vector3(0,0,0);
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

    public Vector3 getRespawnPointPosition()
    {
        return instance.resp.position;
    }

    public void setRespawnPoint(int id, Vector3 pos, Action action)
    {
        if (id >= instance.resp.id && instance.resp.position != pos)
        {
            Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();

            foreach (Checkpoint check in checkpoints)
            {
                if(check.getId() == instance.resp.id)
                {
                    if (check.getId() < id)
                    {
                        check.setDisabled();
                    }
                    else
                    {
                        check.setNotActive();
                    }
                }

            }

            instance.resp.id = id;
            instance.resp.position = pos;
            action.Invoke();
        }
        else if (id == instance.resp.id && instance.resp.position == pos)
        {
            Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();

            foreach (Checkpoint check in checkpoints)
            {
                if (check.getId() < instance.resp.id)
                {
                    check.setDisabled();
                }
            }

            action.Invoke();
        }
    }
}
