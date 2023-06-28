using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;
using System;

public class PaperScrapManager : MonoBehaviour
{
    [System.Serializable]
    private class Scrap
    {
        public int id = 0;
        public bool fromEnemy = false;
        public Vector3 position = new(0, 0, 0);
        public bool collected = false;

        public Scrap(int _id, bool _enemy, Vector3 _pos)
        {
            id = _id;
            fromEnemy = _enemy;
            position = _pos;
        }
    };

    private static PaperScrapManager instance = null;

    [SerializeField] private int collected = 0;
    [SerializeField] private int allPaperScraps = 0;
    [SerializeField] private AudioClip collectedClip;
    [SerializeField] private TMP_Text text;
    [SerializeField] private IPuzzleSolvedEvent puzzleSolvedEvent;

    private List<Scrap> scraps;
    private bool restart;
    private bool start;

    void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance.start = true;
            return;
        }
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
        instance.restart = false;
        instance.start = false;

        scraps = new();
        allPaperScraps = 0;
        collected = 0;

        EnemyController[] arr = FindObjectsOfType<EnemyController>();

        for (int i = 0; i < arr.Length; ++i)
        {
            if (arr[i].HavePaperScrap())
            {
                scraps.Add(new Scrap(allPaperScraps, true, arr[i].gameObject.transform.position));
                arr[i].SetPaperScrapId(allPaperScraps);
                allPaperScraps += 1;
            }
        }

        PaperScrap[] pss = FindObjectsOfType<PaperScrap>();

        for (int i = 0; i < pss.Length; ++i)
        {
            scraps.Add(new Scrap(allPaperScraps + i, false, pss[i].gameObject.transform.position));
            pss[i].SetId(allPaperScraps + i);
        }

        allPaperScraps += pss.Length;

        text.text = new StringBuilder("").Append(collected).Append('/').Append(allPaperScraps).ToString();
    }

    private void Update()
    {
        if (instance.start && instance.restart)
        {
            instance.restart = false;
            instance.start = false;

            scraps.Clear();
            allPaperScraps = 0;
            collected = 0;

            EnemyController[] arr = FindObjectsOfType<EnemyController>();

            for (int i = 0; i < arr.Length; ++i)
            {
                if (arr[i].HavePaperScrap())
                {
                    scraps.Add(new Scrap(allPaperScraps, true, arr[i].gameObject.transform.position));
                    arr[i].SetPaperScrapId(allPaperScraps);
                    allPaperScraps += 1;
                }
            }

            PaperScrap[] pss = FindObjectsOfType<PaperScrap>();

            for (int i = 0; i < pss.Length; ++i)
            {
                scraps.Add(new Scrap(allPaperScraps + i, false, pss[i].gameObject.transform.position));
                pss[i].SetId(allPaperScraps + i);
            }

            allPaperScraps += pss.Length;

            puzzleSolvedEvent = FindObjectOfType<IPuzzleSolvedEvent>();
            text = GameObject.FindGameObjectWithTag("PaperScarpCounterText").GetComponent<TMP_Text>();
            text.text = new StringBuilder("").Append(collected).Append('/').Append(allPaperScraps).ToString();
        }
        else if (instance.start)
        {
            instance.start = false;
            puzzleSolvedEvent = FindObjectOfType<IPuzzleSolvedEvent>();
            text = GameObject.FindGameObjectWithTag("PaperScarpCounterText").GetComponent<TMP_Text>();
            text.text = new StringBuilder("").Append(collected).Append('/').Append(allPaperScraps).ToString();

            EnemyController[] arr = FindObjectsOfType<EnemyController>();

            for (int i = 0; i < arr.Length; ++i)
            {
                if (arr[i].HavePaperScrap())
                {
                    if (CheckIsCollected(new Scrap(0, true, arr[i].gameObject.transform.position)))
                    {
                        arr[i].SetIsPaperScrap(false);
                    }
                    else
                    {
                        SetProperId(new Scrap(0, true, arr[i].gameObject.transform.position), arr[i].SetPaperScrapId);
                    }
                }
            }

            PaperScrap[] pss = FindObjectsOfType<PaperScrap>();

            for (int i = 0; i < pss.Length; ++i)
            {
                if (CheckIsCollected(new Scrap(0, false, pss[i].gameObject.transform.position)))
                {
                    Destroy(pss[i].gameObject);
                }
                else
                {
                    SetProperId(new Scrap(0, false, pss[i].gameObject.transform.position), pss[i].SetId);
                }
            }
        }
    }

    private void SetProperId(Scrap s, Action<int> a)
    {
        foreach (var item in scraps)
        {
            if (item.fromEnemy == s.fromEnemy && item.position == s.position)
            {
                a(item.id);
                return;
            }
        }
    }

    private bool CheckIsCollected(Scrap s)
    {
        foreach (var item in scraps)
        {
            if (item.fromEnemy == s.fromEnemy && item.position == s.position)
            {
                return item.collected;
            }
        }

        return false;
    }

    public void Collected(PaperScrap paper)
    {
        collected += 1;
        scraps[paper.GetId()].collected = true;
        text.text = new StringBuilder("").Append(collected).Append('/').Append(allPaperScraps).ToString();
        paper.PlayAudio(collectedClip);

        if (AreAllCollected())
        {
            if (puzzleSolvedEvent != null)
            {
                puzzleSolvedEvent.Solved();
            }
        }
    }

    public void Restart()
    {
        instance.restart = true;
    }

    public bool AreAllCollected()
    {
        return collected == allPaperScraps;
    }
}
