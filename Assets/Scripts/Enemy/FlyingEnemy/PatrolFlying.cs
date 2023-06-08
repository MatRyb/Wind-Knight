using System.Collections.Generic;
using UnityEngine;

public class PatrolFlying : MonoBehaviour
{
    [SerializeField] private GameObject body;
    [SerializeField] private List<Vector2> points;
    [SerializeField] private float speed = 10.0f;
    private LTDescr tween = null;
    private Matrix4x4 localToWorld;

    void OnValidate()
    {
        localToWorld = transform.localToWorldMatrix;
    }

    void Start()
    {
        if (points.Count == 0)
        {
            Debug.LogError("PatrolFlying -> There is needed at least one point");
        }
        else
        {
            GetComponent<Rigidbody2D>().freezeRotation = true;
            MoveToPoint(0);
        }
    }

    private void MoveToPoint(int i)
    {
        float flyTime = Vector3.Distance(localToWorld.MultiplyPoint3x4(points[i]), this.transform.position) / speed;

        Vector2 dir = (Vector2) localToWorld.MultiplyPoint3x4(points[i]) - (Vector2) this.transform.position;
        if ((dir.x > 0 && body.transform.localScale.x < 0) || (dir.x < 0 && body.transform.localScale.x > 0))
        {
            body.transform.localScale = new Vector2(body.transform.localScale.x * -1, body.transform.localScale.y);
        }

        tween = LeanTween.move(this.gameObject, (Vector2) localToWorld.MultiplyPoint3x4(points[i]), flyTime)
            .setOnComplete(() => { MoveToPoint((++i) % points.Count); });
    }

    public void StopPatrol()
    {
        LeanTween.cancel(tween.id);
        this.enabled = false;
    }

    private void Update()
    {
        if (tween == null)
            return;

        if (GameTimer.TimeMultiplier == GameTimer.STOPPED)
        {
            LeanTween.pause(tween.id);
        }
        else
        {
            LeanTween.resume(tween.id);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (points.Count > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere((Vector2) localToWorld.MultiplyPoint3x4(points[0]), 0.5f);
            for (int i = 1; i < points.Count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere((Vector2) localToWorld.MultiplyPoint3x4(points[i]), 0.5f);

                Gizmos.color = Color.green;
                Gizmos.DrawLine((Vector2) localToWorld.MultiplyPoint3x4(points[i - 1]), (Vector2) localToWorld.MultiplyPoint3x4(points[i]));
            }
            Gizmos.DrawLine((Vector2) localToWorld.MultiplyPoint3x4(points[^1]), (Vector2) localToWorld.MultiplyPoint3x4(points[0]));
        }
    }
}
