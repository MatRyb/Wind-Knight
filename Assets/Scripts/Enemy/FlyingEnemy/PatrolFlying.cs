using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolFlying : MonoBehaviour
{

    [SerializeField] private List<Transform> points;
    [SerializeField] private float speed = 10.0f;
    private LTDescr tween;

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
        float flyTime = Vector3.Distance(points[i].transform.position, this.gameObject.transform.position) / speed;
        tween = LeanTween.move(this.gameObject, points[i].transform.position, flyTime)
            .setOnComplete(() => { moveToPoint((++i)%points.Count); });
    }


    private void Update()
    {
        if (GameTimer.timeMultiplayer == 0)
        {
            LeanTween.pause(tween.id);
        }
        else
        {
            LeanTween.resume(tween.id);
        }
    }
}
