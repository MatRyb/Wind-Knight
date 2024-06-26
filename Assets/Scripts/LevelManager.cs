using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using NaughtyAttributes;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    private class RespawnPoint
    {
        public int id = 0;
        public Vector3 position = new(0,0,0);


        public RespawnPoint(int _id, Vector3 _pos)
        {
            id = _id;
            position = _pos;
        }
    };

    public static LevelManager instance = null;

    private bool waitingForStart = false;
    private GameObject startText = null;

    private bool waitForRespawn = false;

    private bool waitAfterWin = false;

    private bool paused = false;

    private bool start;

    private float timer = 0;
    private bool timerStopped = true;
    public float finalLevelTimer = -1;

    private RespawnPoint startResp = null;
    private RespawnPoint resp = null;

    [SerializeField] [Scene] private string menuScene = "Menu";
    [SerializeField] private AudioClip checkpointClip;
    [SerializeField] private bool isNextLevel = false;
    [SerializeField] [ShowIf("isNextLevel")] [Scene] private string nextLevelSceneName;
    [Scene] public string ThisLevelName = "";

    void OnLevelWasLoaded(int level)
    {
        if (level != SceneManager.GetSceneByName(ThisLevelName).buildIndex)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance.start = true;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            if (instance.ThisLevelName != SceneManager.GetActiveScene().name && ThisLevelName == SceneManager.GetActiveScene().name)
            {
                Destroy(instance.gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        instance.start = true;
        instance.startResp = new(0, GameObject.FindGameObjectWithTag("Player").transform.position);
    }

    void Update()
    {
        if (timer > -1 && !timerStopped)
        {
            timer += Time.deltaTime;
        }
        if (instance.start)
        {
            instance.start = false;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponentInChildren<TrailRenderer>().enabled = false;
            if (instance.resp != null)
            {
                Camera.main.transform.position = new Vector3(instance.resp.position.x, instance.resp.position.y, Camera.main.transform.position.z);
                player.transform.position = new(instance.resp.position.x, instance.resp.position.y, player.transform.position.z);
            }
            else
            {
                Camera.main.transform.position = new Vector3(instance.startResp.position.x, instance.startResp.position.y, Camera.main.transform.position.z);
                player.transform.position = new(instance.startResp.position.x, instance.startResp.position.y, player.transform.position.z);
            }
            player.GetComponentInChildren<PlayerControler>().mouseInit();
            player.GetComponentInChildren<TrailRenderer>().enabled = true;
            PauseGame(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            waitingForStart = true;
            startText = GUIManager.ShowText("Press   to Start");
        }

        if (waitingForStart && Input.GetMouseButtonDown(0))
        {
            StartGame();
        }

        if (waitingForStart)
            return;

        if (waitForRespawn)
        {
            timerStopped = true;
            return;
        }


        if (waitAfterWin)
        {
            if(finalLevelTimer < 0) finalLevelTimer = timer;
            Debug.Log(finalLevelTimer);
            return;
        }

        /*
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
        */

        if (Input.GetKeyDown(KeyCode.Escape) && !paused)
        {
            PauseGameJob(true);
        }
    }

    public void SetTimerToStopped()
    {
        timerStopped = true;
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
                buttons[i].onClick.AddListener(() => instance.RestartLevelJob(true));
            }
            else if (buttons[i].name == "MenuBtn")
            {
                buttons[i].onClick.AddListener(() => SceneManager.LoadScene(instance.menuScene));
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
        timerStopped = true;
        waitForRespawn = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void RestartLevel(bool isScreen)
    {
        instance.RestartLevelJob(isScreen);
    }

    private void RestartLevelJob(bool isScreen)
    {
        timer = 0;
        if (isScreen)
        {
            GameObject screen = GUIManager.ShowRestartQuestionScreen();
            Button[] buttons = screen.GetComponentsInChildren<Button>();

            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].name == "YesBtn")
                {
                    buttons[i].onClick.AddListener(() => instance.RestartYesJob());
                }
                else if (buttons[i].name == "NoBtn")
                {
                    buttons[i].onClick.AddListener(() => instance.RestartNoJob(isScreen));
                }
            }
        }
    }

    private void RestartYesJob()
    {
        timerStopped = true;
        waitForRespawn = false;
        if (instance.resp == null)
        {
            instance.resp = new(startResp.id, startResp.position);
        }
        else
        {
            instance.resp.id = startResp.id;
            instance.resp.position = startResp.position;
        }
        FindObjectOfType<PaperScrapManager>().Restart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void RestartNoJob(bool isScreen)
    {
        if (isScreen)
        {
            GUIManager.HideRestartQuestionScreen();
        }
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
                buttons[i].onClick.AddListener(() => instance.RestartLevelJob(true));
            }
            else if (buttons[i].name == "NextLevelBtn")
            {
                if (!instance.isNextLevel)
                {
                    buttons[i].gameObject.SetActive(false);
                }
                else
                {
                    buttons[i].onClick.AddListener(() => SceneManager.LoadScene(instance.nextLevelSceneName));
                }
            }
            else if (buttons[i].name == "MenuBtn")
            {
                buttons[i].onClick.AddListener(() => SceneManager.LoadScene(instance.menuScene));
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
        timerStopped = false;
        ResumeGame(false);
    }

    public static void PauseGame(bool isScreen)
    {
        instance.PauseGameJob(isScreen);
    }

    private void PauseGameJob(bool isScreen)
    {
        timerStopped = true;
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
                    buttons[i].onClick.AddListener(() => instance.RestartLevelJob(true));
                }
                else if (buttons[i].name == "ResumeBtn")
                {
                    buttons[i].onClick.AddListener(() => instance.ResumeGameJob(isScreen));
                }
                else if (buttons[i].name == "MenuBtn")
                {
                    buttons[i].onClick.AddListener(() => SceneManager.LoadScene(instance.menuScene));
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
        timerStopped = false;
        GameTimer.StartTime();
        paused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (isScreen)
        {
            GUIManager.HidePauseScreen();
        }
    }

    public Vector3 GetRespawnPointPosition()
    {
        if (instance.resp != null)
        {
            return instance.resp.position;
        }
        else
        {
            return instance.startResp.position;
        }
    }

    public void SetRespawnPoint(int id, Vector3 pos, Action<AudioClip, bool> action)
    {
        if (instance.resp == null || (id >= instance.resp.id && instance.resp.position != pos))
        {
            Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();

            foreach (Checkpoint check in checkpoints)
            {
                if (instance.resp == null)
                {
                    if (check.GetId() < id)
                    {
                        check.SetDisabled();
                    }
                    else
                    {
                        check.SetNotActive();
                    }
                }
                else
                {
                    if (check.GetId() <= instance.resp.id)
                    {
                        if (check.GetId() < id)
                        {
                            check.SetDisabled();
                        }
                        else
                        {
                            check.SetNotActive();
                        }
                    }
                }
            }

            if (instance.resp == null)
            {
                instance.resp = new(id, pos);
            }
            else
            {
                instance.resp.id = id;
                instance.resp.position = pos;
            }
            action.Invoke(checkpointClip, false);
            
        }
        else if (id == instance.resp.id && instance.resp.position == pos)
        {
            Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();

            foreach (Checkpoint check in checkpoints)
            {
                if (check.GetId() < instance.resp.id)
                {
                    check.SetDisabled();
                }
            }

            action.Invoke(checkpointClip, true);
        }
    }
}
