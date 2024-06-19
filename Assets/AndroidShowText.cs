using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AndroidShowText : MonoBehaviour
{
    public TextMeshProUGUI text;

    public Image windowsImage;

    void Start()
    {
#if UNITY_ANDROID
        windowsImage.gameObject.SetActive(false);
        text.text = "Press to Start";
#elif UNITY_STANDALONE
        windowsImage.gameObject.SetActive(true);
        text.text = "Press   to Start";
#endif
    }
}
