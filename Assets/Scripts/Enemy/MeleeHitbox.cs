using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    [SerializeField] private SpriteRenderer renderer;


    [SerializeField] private float durationOfOutline;
    [SerializeField] private float dieTime;
    [SerializeField] private float damage;
    private LocalTimerContainer timer;
    private LocalTimerContainer timer2;
    private LocalTimerContainer timer3;

    [SerializeField] private Color deafultColor = new((87f / 255f), 0, 0, 10);
    [SerializeField] private Color blinkColor = new(100, 0, 0, 10);

    private void OnValidate()
    {
        if (renderer == null && GetComponent<SpriteRenderer>() != null)
        {
            renderer = GetComponent<SpriteRenderer>();
        }
        else if (renderer == null && GetComponent<SpriteRenderer>() == null)
        {
            Debug.LogError(gameObject.name + " Sprite Renderer doesn't provided!!!");
        }
    }

    void Start()
    {
        GetComponent<Collider2D>().enabled = false;
        //GetComponent<SpriteRenderer>().color = Color.white;

        timer3 = LocalTimersManager.CreateNewTimer(0.01f).DoAfter(() =>
            {
                LeanTween.value(renderer.gameObject, SetSpriteColor, renderer.color, blinkColor, durationOfOutline).setOnComplete(() =>
                {
                    LeanTween.value(renderer.gameObject, SetSpriteColor, renderer.color, deafultColor, durationOfOutline);
                });
            }).Start();

        timer = LocalTimersManager.CreateNewTimer(durationOfOutline).DoAfter(() => 
        {

            Deploy();
        }).Start();
    }

    void Deploy() {
        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().color = Color.red;

        if (gameObject != null)
        {
            timer2 = LocalTimersManager.CreateNewTimer(dieTime).DoAfter(() => Destroy(gameObject)).Start();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerControler _))
        {
            if (collision.transform.gameObject.TryGetComponent(out IDamageTaker damageTaker))
            {
                damageTaker.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        timer?.Stop();
        timer2?.Stop();
        timer3?.Stop();
    }

    public void SetSpriteColor(Color val)
    {
        renderer.color = val;
    }
}
