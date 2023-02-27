using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int id;
    private LevelManager lvl;
    private SpriteRenderer sprite;

    private Color activeColor = new Color(117f / 255f, 205f / 255f, 94f / 255f);
    private Color notActiveColor = new Color(106f / 255f, 106f / 255f, 106f / 255f);
    private Color disabledColor = new Color(205f / 255f, 94f / 255f, 94f / 255f);

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
        lvl.setRespawnPoint(id, gameObject.transform.position, setActive);
    }

    public void setActive()
    {
        sprite.color = activeColor;
    }

    public void setNotActive()
    {
        sprite.color = notActiveColor;
    }

    public void setDisabled()
    {
        sprite.color = disabledColor;
    }

    public int getId()
    {
        return id;
    }
}
