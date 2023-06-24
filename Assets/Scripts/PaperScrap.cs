using UnityEngine;

public class PaperScrap : MonoBehaviour
{
    private PaperScrapManager manager;

    private int id = 0;

    private void Awake()
    {
        if (manager == null)
        {
            manager = FindObjectOfType<PaperScrapManager>();
        }
    }

    public void SetId(int value)
    {
        id = value;
    }

    public int GetId()
    {
        return id;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerControler _))
        {
            manager.Collected(this);
            Destroy(gameObject);
        }
    }
}
