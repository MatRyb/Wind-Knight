using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AndroidInfo : MonoBehaviour
{
    public TextMeshProUGUI text;

    [Multiline] public string windows;
    [Multiline] public string android;

    public Image windowsImage;

    void Start()
    {
#if UNITY_ANDROID
        text.text = android;
        windowsImage.gameObject.SetActive(false);
#elif UNITY_STANDALONE
        text.text = windows;
        windowsImage.gameObject.SetActive(true);
#endif
    }
}
