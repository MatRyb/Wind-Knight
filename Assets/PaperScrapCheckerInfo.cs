using UnityEngine;
using TMPro;
using System.Text;

public class PaperScrapCheckerInfo : MonoBehaviour
{
    [SerializeField] private Transform spawn;
    [SerializeField] [Tooltip("%n - number of missing paper scraps, %N - number of needed paper scraps")] private string info;
    [SerializeField] private GameObject notePrefab;

    private GameObject infoObj = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerControler _))
        {
            if (!PaperScrapManager.instance.AreAllCollected())
            {
                infoObj = Instantiate(notePrefab, spawn.position, new Quaternion(0, 0, 0, 0), spawn);

                StringBuilder s = new("");

                char buff = ' ';

                foreach (char c in info)
                {
                    if (c == '%')
                    {
                        buff = c;
                        continue;
                    }

                    string temp = new StringBuilder("").Append(buff).Append(c).ToString();

                    if (temp == "%n")
                    {
                        s.Append(PaperScrapManager.instance.GetNeededCollectedNumber() - PaperScrapManager.instance.GetCollectedNumber());
                        buff = ' ';
                    }
                    else if (temp == "%N")
                    {
                        s.Append(PaperScrapManager.instance.GetNeededCollectedNumber());
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

                infoObj.GetComponentInChildren<TMP_Text>().text = s.ToString();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerControler _))
        {
            if (infoObj != null && PaperScrapManager.instance.AreAllCollected())
            {
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
                Destroy(infoObj);
                infoObj = null;
            }
        }
    }
}
