using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager instance;

    [Header("Music:")]
    [SerializeField] private Slider music;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private TMP_Text musicText;
    [Range(0, 100)] [SerializeField] private float changeTextColorMusicValue = 50f;
    [SerializeField] private Image muteMusicIcon;

    [Header("SFX:")]
    [SerializeField] private Slider sfx;
    [SerializeField] private TMP_Text sfxText;
    [SerializeField] private bool sfxMute;
    [Range(0, 100)] [SerializeField] private float changeTextColorSFXValue = 50f;
    [SerializeField] private Image muteSFXIcon;

    [Header("Global:")]
    [SerializeField] private Color textWhenBackground;
    [SerializeField] private Color textWhenForeground;
    [SerializeField] private Sprite muteSprite;
    [SerializeField] private Sprite unMuteSprite;

    void Awake()
    {
        instance = this;

        // music
        music.onValueChanged.AddListener(delegate { ChangeVolume(musicSource, music.value, musicText, changeTextColorMusicValue); PlayerPrefs.SetFloat("MinMusicVolume", music.value - 10f); PlayerPrefs.SetFloat("MaxMusicVolume", music.value + 10f); });
        music.value = PlayerPrefs.GetFloat("MinMusicVolume", 60f) + 10f;
        musicSource.mute = PlayerPrefs.GetInt("MuteMusicVolume", 0) != 0;
        CheckAudioMute(musicSource.mute, muteMusicIcon);

        //sfx
        sfx.onValueChanged.AddListener(delegate { ChangeVolume(sfx.value, sfxText, changeTextColorSFXValue); PlayerPrefs.SetFloat("SFXVolume", sfx.value); });
        sfx.value = PlayerPrefs.GetFloat("SFXVolume", 40f);
        sfxMute = PlayerPrefs.GetInt("MuteSFXVolume", 0) != 0;
        CheckAudioMute(sfxMute, muteSFXIcon);
    }

    public void ChangeVolume(AudioSource audioSource, float volume, TMP_Text tekst, float changeTextColorValue)
    {
        tekst.text = new StringBuilder("").Append((int)(volume - 10)).Append('-').Append((int)(volume + 10)).Append('%').ToString();
        if(volume < changeTextColorValue)
        {
            tekst.color = textWhenBackground;
        }
        else
        {
            tekst.color = textWhenForeground;
        }
        audioSource.volume = volume / 100f;
    }

    public void ChangeVolume(float volume, TMP_Text tekst, float changeTextColorValue)
    {
        tekst.text = new StringBuilder("").Append((int)volume).Append('%').ToString();
        if (volume < changeTextColorValue)
        {
            tekst.color = textWhenBackground;
        }
        else
        {
            tekst.color = textWhenForeground;
        }
    }

    public void MuteMusicAudio()
    {
        musicSource.mute = !musicSource.mute;
        PlayerPrefs.SetInt("MuteMusicVolume", musicSource.mute ? 1 : 0);
        CheckAudioMute(musicSource.mute, muteMusicIcon);
    }

    public void MuteSFXAudio()
    {
        sfxMute = !sfxMute;
        PlayerPrefs.SetInt("MuteSFXVolume", sfxMute ? 1 : 0);
        CheckAudioMute(sfxMute, muteSFXIcon);
    }

    private void CheckAudioMute(bool audioMute, Image icon)
    {
        if (audioMute)
        {
            icon.sprite = muteSprite;
        }
        else
        {
            icon.sprite = unMuteSprite;
        }
    }
}
