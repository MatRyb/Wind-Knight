using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaMonitor : MonoBehaviour
{
    [SerializeField] private PlayerControler manaMonitor;
    [SerializeField] private Image image;
    void Update()
    {
        image.fillAmount = (float)manaMonitor.GetMana() / (float)manaMonitor.GetMaxMana();
    }
}
