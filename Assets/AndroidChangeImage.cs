using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AndroidChangeImage : MonoBehaviour
{
    public Image android;
    public Image windows;

    void Start()
    {
#if UNITY_ANDROID
        android.gameObject.SetActive(true);
        windows.gameObject.SetActive(false);
#elif UNITY_STANDALONE
        android.gameObject.SetActive(false);
        windows.gameObject.SetActive(true);
#endif
    }
}
