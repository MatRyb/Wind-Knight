using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsLevelManager : MonoBehaviour
{
    public static OptionsLevelManager instance;

    [Header("Music:")]
    public AudioSource musicSource;

    void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            Destroy(gameObject);
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

        musicSource.volume = PlayerPrefs.GetFloat("Volume", 70f) / 100f;
    }
}
