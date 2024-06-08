using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public enum MoveType {
    Touch = 0,
    Joystick = 1
}

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

    [Header("Android:")]
    [SerializeField] private GameObject moveOptions;
    [SerializeField] private MoveType moveType;
    [SerializeField] private Toggle touch;
    [SerializeField] private Toggle joystick;

    void Awake()
    {
        instance = this;

        // music
        music.onValueChanged.AddListener(delegate { ChangeVolume(musicSource, music.value, musicText, changeTextColorMusicValue); PlayerPrefs.SetFloat("MinMusicVolume", music.value - 10f); PlayerPrefs.SetFloat("MaxMusicVolume", music.value + 10f); });
        music.value = PlayerPrefs.GetFloat("MinMusicVolume", 60f) + 10f;
        musicSource.mute = PlayerPrefs.GetInt("MuteMusicVolume", 0) != 0;
        CheckAudioMute(musicSource.mute, muteMusicIcon);

        // sfx
        sfx.onValueChanged.AddListener(delegate { ChangeVolume(sfx.value, sfxText, changeTextColorSFXValue); PlayerPrefs.SetFloat("SFXVolume", sfx.value); });
        sfx.value = PlayerPrefs.GetFloat("SFXVolume", 40f);
        sfxMute = PlayerPrefs.GetInt("MuteSFXVolume", 0) != 0;
        CheckAudioMute(sfxMute, muteSFXIcon);

        // android movement
#if UNITY_ANDROID
        moveOptions.SetActive(true);
#else
        moveOptions.SetActive(false);
#endif
    }

    void Start()
    {
#if UNITY_ANDROID
        CheckMoveOption();
#endif
    }

    public void CheckMoveOption()
    {
        MoveType type = (MoveType)PlayerPrefs.GetInt("AndroidMoveType", (int)MoveType.Joystick);

        if (moveType != type)
        {
            moveType = type;
            if (moveType == MoveType.Joystick)
            {
                touch.isOn = false;
                joystick.isOn = true;
            }
            else if (moveType == MoveType.Touch)
            {
                joystick.isOn = false;
                touch.isOn = true;
            }
        }
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

    public void SetAndroidMovement(MoveType type)
    {
        if (moveType != type)
        {
            moveType = type;
            PlayerPrefs.SetInt("AndroidMoveType", (int)type);
        }
    }

    public void SetAndroidTouch()
    {
        SetAndroidMovement(MoveType.Touch);
    }

    public void SetAndroidJoystick()
    {
        SetAndroidMovement(MoveType.Joystick);
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
