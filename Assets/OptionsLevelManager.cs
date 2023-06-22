using UnityEngine;

public class OptionsLevelManager : MonoBehaviour
{
    public static OptionsLevelManager instance;

    [Header("Player:")]
    [SerializeField] private PlayerControler player;

    [Header("Music:")]
    [SerializeField] private AudioSource musicSource;

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

        musicSource.volume = PlayerPrefs.GetFloat("MinVolume", 60f) / 100f;

        musicSource.mute = PlayerPrefs.GetInt("MuteVolume", 0) == 0 ? false : true;
    }

    void Update()
    {
        float x = (player.speed - player.minSpeed) / (player.maxSpeed - player.minSpeed);
        float min = PlayerPrefs.GetFloat("MinVolume", 60f);
        float max = PlayerPrefs.GetFloat("MaxVolume", 80f);
        musicSource.volume = ((max - min) * x + min) / 100f;
    }
}
