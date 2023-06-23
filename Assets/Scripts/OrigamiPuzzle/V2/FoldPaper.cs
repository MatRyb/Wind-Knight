using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoldPaper : MonoBehaviour
{
    public int orderIndex = -1;
    public List<GameObject> corners;

    public OrigamiUIPuzzleManager2 manager;

    public Vector3 rotAxis;
    public float rotDuration;
    public float rotAngle;

    public bool rotated = false;

    public void RotatePaper(bool back)
    {
        foreach(var c in corners)
        {
            c.transform.parent = gameObject.transform;
        }

        if (!back)
        {
            orderIndex = manager.currentOrderIndex;
            manager.foldPapers.Add(this);
            StartCoroutine(FoldAroundAxis(gameObject.transform, rotAxis, rotAngle, transform.position - new Vector3(0, 0, orderIndex * 0.001f + 0.001f), rotDuration, back));
            rotated = true;
        }
        else
        {
            StartCoroutine(FoldAroundAxis(gameObject.transform, rotAxis, -rotAngle, transform.position + new Vector3(0, 0, orderIndex * 0.001f + 0.001f), rotDuration, back));
            orderIndex = -1;
            manager.foldPapers.Remove(this);
            rotated = false;
        }
    }

    IEnumerator FoldAroundAxis(Transform paperTransform, Vector3 axis, float angleChange, Vector3 endPosition, float duration, bool back)
    {
        manager.clickeBlocked = true;

        Quaternion startRotation = paperTransform.rotation;
        Vector3 startPosition = paperTransform.position;

        float t = 0;
        while (t < duration)
        {
            paperTransform.rotation = startRotation * Quaternion.AngleAxis(angleChange * manager.rotCurve.Evaluate(t / duration), axis);
            paperTransform.position = Vector3.Lerp(startPosition, endPosition, t / duration);

            t += Time.deltaTime;
            yield return null;
        }

        paperTransform.rotation = startRotation * Quaternion.AngleAxis(angleChange, axis);
        paperTransform.position = endPosition;

        if (back)
        {
            manager.currentOrderIndex--;
        }
        else
        {
            manager.currentOrderIndex++;
        }

        manager.clickeBlocked = false;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
