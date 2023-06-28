using UnityEngine;
using UnityEngine.UI;

public class ManaMonitor : MonoBehaviour
{
    [SerializeField] private PlayerControler manaToMonitor;
    [SerializeField] private Image image;
    void Update()
    {
        image.fillAmount = (float)manaToMonitor.GetMana() / (float)manaToMonitor.GetMaxMana();
    }
}
