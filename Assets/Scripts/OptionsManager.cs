using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager instance;

    [Header("Music:")]
    public Slider music;
    public AudioSource musicSource;
    public TMP_Text musicText;
    [Range(0, 100)] public float changeTextColorValue = 50f;
    public Color text50;
    public Color text100;

    void Awake()
    {
        instance = this;

        music.onValueChanged.AddListener(delegate { ChangeVolume(musicSource, music.value, musicText); PlayerPrefs.SetFloat("Volume", music.value); });
    }

    public void ChangeVolume(AudioSource audioSource, float volume, TMP_Text tekst)
    {
        tekst.text = (int)volume + "%";
        if(volume < changeTextColorValue)
        {
            tekst.color = text50;
        }
        else
        {
            tekst.color = text100;
        }
        audioSource.volume = volume / 100f;
    }
}
