using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int id;
    private LevelManager lvl;
    private SpriteRenderer sprite;

    private Color activeColor = new(117f / 255f, 205f / 255f, 94f / 255f);
    private Color notActiveColor = new(106f / 255f, 106f / 255f, 106f / 255f);
    private Color disabledColor = new(205f / 255f, 94f / 255f, 94f / 255f);

    void Awake()
    {
        if (lvl == null)
        {
            lvl = FindObjectOfType<LevelManager>();
        }

        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerControler>(out PlayerControler _))
        {
            lvl.SetRespawnPoint(id, gameObject.transform.position, SetActive);
        }
    }

    public void SetActive()
    {
        sprite.color = activeColor;
    }

    public void SetNotActive()
    {
        sprite.color = notActiveColor;
    }

    public void SetDisabled()
    {
        sprite.color = disabledColor;
    }

    public int GetId()
    {
        return id;
    }
}
