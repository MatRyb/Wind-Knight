using UnityEngine;

public class PaperScrap : MonoBehaviour
{
    private PaperScrapManager manager;

    [SerializeField] private GameObject source;

    private int id = 0;

    private float volume = 0f;
    private bool mute = false;

    private void Start()
    {
        if (manager == null)
        {
            manager = FindObjectOfType<PaperScrapManager>();
        }

        volume = OptionsLevelManager.instance.GetSFXVolume();
        mute = OptionsLevelManager.instance.GetSFXMute();
    }

    public void SetId(int value)
    {
        id = value;
    }

    public PaperScrap SetVolume(float value)
    {
        volume = value;
        return this;
    }

    public PaperScrap SetMute(bool value)
    {
        mute = value;
        return this;
    }

    public int GetId()
    {
        return id;
    }

    public void PlayAudio(AudioClip clip)
    {
        AudioSource s = Instantiate(source, transform.position, new Quaternion(0,0,0,0)).GetComponent<AudioSource>();
        s.clip = clip;
        s.volume = volume;
        s.mute = mute;
        s.Play();
        Destroy(s.gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerControler _))
        {
            if (manager == null)
            {
                manager = FindObjectOfType<PaperScrapManager>();
            }

            manager?.Collected(this);
            Destroy(gameObject);
        }
    }
}
