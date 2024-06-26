using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class OptionsLevelManager : MonoBehaviour
{
    public static OptionsLevelManager instance;

    [Header("Player:")]
    [SerializeField] private PlayerControler player;

    [Header("Music:")]
    [SerializeField] private AudioSource musicSource;

    [Header("SFX:")]
    [SerializeField] private List<AudioSource> sfxSources;
    [SerializeField] [Tag] private string sfxTag = "SFX";
    [Scene] public string ThisLevelName = "";

    private bool start;

    void OnLevelWasLoaded(int level)
    {
        if (level != SceneManager.GetSceneByName(ThisLevelName).buildIndex)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance.start = true;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            if (instance.ThisLevelName != SceneManager.GetActiveScene().name && ThisLevelName == SceneManager.GetActiveScene().name)
            {
                Destroy(instance.gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        instance.start = false;

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

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat("SFXVolume", 40f) / 100f;
    }

    public bool GetSFXMute()
    {
        return PlayerPrefs.GetInt("MuteSFXVolume", 0) != 0;
    }

    void Update()
    {
        if (instance.start)
        {
            instance.start = false;

            player = FindObjectOfType<PlayerControler>();

            var arr = GameObject.FindGameObjectsWithTag(sfxTag);

            sfxSources.Clear();

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

        float x = (player.speed - player.minSpeed) / (player.maxSpeed - player.minSpeed);
        float min = PlayerPrefs.GetFloat("MinMusicVolume", 60f);
        float max = PlayerPrefs.GetFloat("MaxMusicVolume", 80f);

        if (GameTimer.TimeMultiplier == GameTimer.PLAYING)
        {
            musicSource.volume = ((max - min) * x + min) / 100f;
        }
        else
        {
            if (musicSource.volume != min / 100f)
            {
                musicSource.volume = min / 100f;
            }
        }
    }
}
