using UnityEngine;
using System.Collections.Generic;

public class OptionsLevelManager : MonoBehaviour
{
    public static OptionsLevelManager instance;

    [Header("Player:")]
    [SerializeField] private PlayerControler player;

    [Header("Music:")]
    [SerializeField] private AudioSource musicSource;

    [Header("SFX:")]
    [SerializeField] private List<AudioSource> sfxSources;
    [SerializeField] private string sfxTag = "SFX";

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

        player = FindObjectOfType<PlayerControler>();

        var arr = GameObject.FindGameObjectsWithTag(sfxTag);

        for (int i = 0; i < arr.Length; ++i)
        {
            if (arr[i].TryGetComponent(out AudioSource s))
            {
                sfxSources.Add(s);
            }
        }

        foreach (var item in sfxSources)
        {
            item.volume = PlayerPrefs.GetFloat("SFXVolume", 40f) / 100f;
            item.mute = PlayerPrefs.GetInt("MuteSFXVolume", 0) != 0;
        }

        musicSource.volume = PlayerPrefs.GetFloat("MinMusicVolume", 60f) / 100f;

        musicSource.mute = PlayerPrefs.GetInt("MuteMusicVolume", 0) != 0;
    }

    void Update()
    {
        float x = (player.speed - player.minSpeed) / (player.maxSpeed - player.minSpeed);
        float min = PlayerPrefs.GetFloat("MinMusicVolume", 60f);
        float max = PlayerPrefs.GetFloat("MaxMusicVolume", 80f);
        musicSource.volume = ((max - min) * x + min) / 100f;
    }
}
