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
    [Range(0, 100)] [SerializeField] private float changeTextColorValue = 50f;
    [SerializeField] private Color textWhenBackground;
    [SerializeField] private Color textWhenForeground;
    [SerializeField] private Image muteIcon;
    [SerializeField] private Sprite muteSprite;
    [SerializeField] private Sprite unMuteSprite;

    void Awake()
    {
        instance = this;

        music.onValueChanged.AddListener(delegate { ChangeVolume(musicSource, music.value, musicText); PlayerPrefs.SetFloat("MinVolume", music.value - 10f); PlayerPrefs.SetFloat("MaxVolume", music.value + 10f); });

        music.value = PlayerPrefs.GetFloat("MinVolume", 60f) + 10f;

        musicSource.mute = PlayerPrefs.GetInt("MuteVolume", 0) == 0 ? false : true;

        CheckAudioSource(musicSource);
    }

    public void ChangeVolume(AudioSource audioSource, float volume, TMP_Text tekst)
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

    public void MuteAudio()
    {
        musicSource.mute = !musicSource.mute;
        PlayerPrefs.SetInt("MuteVolume", musicSource.mute ? 1 : 0);
        CheckAudioSource(musicSource);
    }

    private void CheckAudioSource(AudioSource audioSource)
    {
        if (audioSource.mute)
        {
            muteIcon.sprite = muteSprite;
        }
        else
        {
            muteIcon.sprite = unMuteSprite;
        }
    }
}
