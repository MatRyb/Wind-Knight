using System.Collections.Generic;
using UnityEngine;

public class PatrolFlying : MonoBehaviour
{
    [SerializeField] private GameObject body;
    [SerializeField] private List<Vector2> points;
    [SerializeField] private float speed = 10.0f;
    private LTDescr tween = null;

    void Start()
    {
        if (points.Count == 0)
        {
            Debug.LogError("PatrolFlying -> There is needed at least one point");
        }
        else
        {
            moveToPoint(0);
        }
    }

    private void moveToPoint(int i)
    {
        float flyTime = Vector3.Distance(points[i], this.transform.position) / speed;

        Vector2 dir = points[i] - (Vector2) this.transform.position;
        if ((dir.x > 0 && transform.localScale.x < 0) || (dir.x < 0 && transform.localScale.x > 0))
        {
            body.transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }

        tween = LeanTween.move(this.gameObject, points[i], flyTime)
            .setOnComplete(() => { moveToPoint((++i)%points.Count); });
    }

    private void Update()
    {
        if (tween == null)
            return;

        if (GameTimer.timeMultiplayer == 0)
        {
            LeanTween.pause(tween.id);
        }
        else
        {
            LeanTween.resume(tween.id);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, collision.transform.position - transform.position);
            if (hit.collider.tag == "Player")
            {
                LeanTween.cancel(tween.id);
                this.enabled = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (points.Count > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(points[0], 0.5f);
            for (int i = 1; i < points.Count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(points[i], 0.5f);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(points[i - 1], points[i]);
            }
            Gizmos.DrawLine(points[points.Count - 1], points[0]);
        }
    }
}
