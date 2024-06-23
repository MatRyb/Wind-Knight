using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class HealthMonitor : MonoBehaviour
{
    [SerializeField] private ObjectHealth healthToMonitor;
    [SerializeField] private int maxNumberOfHearts = 4;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite halfHeart;
    [SerializeField] private Sprite zeroHeart;
    [SerializeField] private GameObject heartsHolder;
    [SerializeField] private Image[] hearts;

    private void Start()
    {
        for (int i = 0; i < heartsHolder.transform.childCount; ++i)
        {
            Destroy(heartsHolder.transform.GetChild(i).gameObject);
        }

        hearts = new Image[maxNumberOfHearts];

        for (int i = 0; i < maxNumberOfHearts; ++i) 
        {
            GameObject obj = new(new StringBuilder().Append("Heart_").Append(i).ToString(), typeof(Image));
            obj.transform.parent = heartsHolder.transform;
            obj.transform.localScale = new(1.0f, 1.0f, 1.0f);
            obj.GetComponent<Image>().sprite = fullHeart;
            hearts[i] = obj.GetComponent<Image>();
        }
    }

    void Update()
    {
        //image.fillAmount = healthToMonitor.GetHealth() / healthToMonitor.GetMaxHealth();
        float percent = healthToMonitor.GetHealth() / healthToMonitor.GetMaxHealth();
        float step = 1.0f / ((float)maxNumberOfHearts * 2.0f);
        int halfs = Mathf.FloorToInt(percent / step);

        int halfsCount = halfs;
        for (int i = 0; i < hearts.Length; ++i)
        {
            if (halfsCount - 2 >= 0)
            {
                hearts[i].sprite = fullHeart;
                halfsCount -= 2;
            }
            else if (halfsCount - 1 >= 0)
            {
                hearts[i].sprite = halfHeart;
                halfsCount -= 1;
            }
            else
            {
                hearts[i].sprite = zeroHeart;
            }
        }

        float realHealth = (float)halfs * step * healthToMonitor.GetMaxHealth();
        if (realHealth != healthToMonitor.GetHealth())
        {
            float dmg = realHealth - healthToMonitor.GetHealth();
            healthToMonitor.TakeDamage(dmg < 0.0f ? -dmg : 0.0f);
        }
    }
}
