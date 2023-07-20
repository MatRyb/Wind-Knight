using UnityEngine;
using TMPro;
using System.Text;
using NaughtyAttributes;

public class PaperScrapCheckerInfo : MonoBehaviour
{
    [System.Serializable]
    private class Scrap
    {
        public bool fromEnemy = false;
        public Vector3 position = new(0, 0, 0);

        public Scrap(bool _enemy, Vector3 _pos)
        {
            fromEnemy = _enemy;
            position = _pos;
        }
    };

    [SerializeField] private bool checkOneSpecificScrap = false;
    private bool negativeCheck = true;


    [SerializeField] [ShowIf("checkOneSpecificScrap")] private Scrap toCheck;
    [SerializeField] private Transform spawn;
    [SerializeField] [ShowIf("negativeCheck")] [Tooltip("%n - number of missing paper scraps, %N - number of needed paper scraps, %E - type 'Enter' symbol")] private string info_all;
    [SerializeField] [ShowIf("checkOneSpecificScrap")] [Tooltip("%E - type 'Enter' symbol")] private string info_one;
    [SerializeField] private GameObject notePrefab;

    private GameObject infoObj = null;

    private void OnValidate()
    {
        negativeCheck = !checkOneSpecificScrap;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerControler _))
        {
            if (!checkOneSpecificScrap)
            {
                if (!PaperScrapManager.instance.AreAllCollected())
                {
                    infoObj = Instantiate(notePrefab, spawn.position, new Quaternion(0, 0, 0, 0), spawn);

                    infoObj.GetComponentInChildren<TMP_Text>().text = FillGaps(info_all, false);

                    PaperScrapManager.instance.OnScrapCollected += UpdateInfo;
                }
            }
            else
            {
                if (!PaperScrapManager.instance.CheckIfCollected(toCheck.position, toCheck.fromEnemy))
                {
                    infoObj = Instantiate(notePrefab, spawn.position, new Quaternion(0, 0, 0, 0), spawn);

                    infoObj.GetComponentInChildren<TMP_Text>().text = FillGaps(info_one, true);

                    PaperScrapManager.instance.OnScrapCollected += UpdateInfo;
                }
            }
        }
    }

    private void UpdateInfo()
    {
        if (infoObj != null)
        {
            if (!checkOneSpecificScrap)
            {
                if (!PaperScrapManager.instance.AreAllCollected())
                {
                    infoObj.GetComponentInChildren<TMP_Text>().text = FillGaps(info_all, false);
                }
            }
            else
            {
                if (!PaperScrapManager.instance.CheckIfCollected(toCheck.position, toCheck.fromEnemy))
                {
                    infoObj.GetComponentInChildren<TMP_Text>().text = FillGaps(info_one, true);
                }
            }
        }
    }

    private string FillGaps(string text, bool one)
    {
        StringBuilder s = new("");

        char buff = ' ';

        foreach (char c in text)
        {
            if (c == '%')
            {
                buff = c;
                continue;
            }

            string temp = new StringBuilder("").Append(buff).Append(c).ToString();

            if (temp == "%n")
            {
                if (!one)
                {
                    s.Append(PaperScrapManager.instance.GetNeededCollectedNumber() - PaperScrapManager.instance.GetCollectedNumber());
                    buff = ' ';
                }
                else
                {
                    s.Append(temp);
                    buff = ' ';
                }
            }
            else if (temp == "%N")
            {
                if (!one)
                {
                    s.Append(PaperScrapManager.instance.GetNeededCollectedNumber());
                    buff = ' ';
                }
                else
                {
                    s.Append(temp);
                    buff = ' ';
                }
            }
            else if (temp == "%E")
            {
                s.Append('\n');
                buff = ' ';
            }
            else
            {
                if (buff == '%')
                {
                    s.Append(buff);
                    buff = ' ';
                    s.Append(c);
                }
                else
                {
                    s.Append(c);
                }
            }

        }

        return s.ToString();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerControler _))
        {
            if (infoObj != null && ((!checkOneSpecificScrap && PaperScrapManager.instance.AreAllCollected()) || (checkOneSpecificScrap && PaperScrapManager.instance.CheckIfCollected(toCheck.position, toCheck.fromEnemy))))
            {
                PaperScrapManager.instance.OnScrapCollected -= UpdateInfo;
                Destroy(infoObj);
                infoObj = null;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerControler _))
        {
            if (infoObj != null)
            {
                PaperScrapManager.instance.OnScrapCollected -= UpdateInfo;
                Destroy(infoObj);
                infoObj = null;
            }
        }
    }
}
