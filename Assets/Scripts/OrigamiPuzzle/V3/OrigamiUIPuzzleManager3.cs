using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[System.Serializable]
public struct Line
{
    [SerializeField] private Vector3 pointA;
    [SerializeField] private Vector3 pointB;

    public Vector3 GetPoint(float t)
    {
        return pointA + (pointB - pointA) * t;
    }

    public Vector3 GetAxis()
    {
        return (pointB - pointA).normalized;
    }
}

[System.Serializable]
public struct FoldData
{
    public Line line;
    public float angle;
}

[System.Serializable]
public struct FoldMove
{
    public List<GameObject> stateBefore;
    public List<GameObject> stateAfter;
    public List<GameObject> rotatedObjects;
    public int foldData;
}

public class OrigamiUIPuzzleManager3 : MonoBehaviour
{
    [SerializeField] private Camera UICamera = null;
    [SerializeField] private GameObject UIObject = null;
    [SerializeField] private GameObject PaperObject = null;
    private GameObject RotateHandler = null;

    public List<GameObject> currentPaperSlices = new();

    public List<FoldMove> movesHistory = new();
    public List<FoldData> folds = new();
    public List<int> disabledFolds = new();

    public AnimationCurve rotCurve = new();

    public bool clickeBlocked = false;

    private void Start()
    {
        RotateHandler = new("RotateHandler");
        RotateHandler.transform.parent = PaperObject.transform;
        RotateHandler.transform.localScale = Vector3.one;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !clickeBlocked)
        {
            if (Physics.Raycast(UICamera.ScreenToWorldPoint(Input.mousePosition), UICamera.transform.forward, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out FoldPaperClicker clicker))
                {
                    int foldIndex = clicker.foldIndex;
                    if (!disabledFolds.Contains(foldIndex))
                    {
                        FoldPaper(foldIndex);
                        disabledFolds.Add(foldIndex);
                    }
                    /*else
                    {
                        if (foldPaper.orderIndex < currentOrderIndex - 1)
                        {
                            StartCoroutine(RotateBackToIndex(foldPaper.orderIndex));
                        }
                        else
                        {
                            foldPaper.RotatePaper(true);
                        }
                    }*/
                }
            }
        }
    }

    IEnumerator RotateBackToIndex(int index)
    {
        /*int tmpIndex = currentOrderIndex;
        while (currentOrderIndex != index)
        {
            if (tmpIndex == currentOrderIndex)
            {
                foldPapers[tmpIndex - 1].RotatePaper(true);
                tmpIndex--;
            }
            yield return new WaitForEndOfFrame();
        }*/
        yield return null;
    }

    private void FoldPaper(int foldIndex) 
    {
        // Clear RotateHandler Before Rotate
        var rotateTransforms = RotateHandler.GetComponentsInChildren<Transform>();
        foreach (var obj in rotateTransforms)
        {
            obj.parent = PaperObject.transform;
        }

        List<GameObject> toRotateSlices = new();
        List<GameObject> newSlices = new();
        var fold = folds[foldIndex];
        foreach (var c in currentPaperSlices)
        {
            Plane p = GetPlaneForObjectFromLine(fold.line, c);
            GameObject[] slices = Slicer.Slice(p, c);

            // Check Slices (if one is empty don't create new)

            toRotateSlices.Add(slices[0]);
            newSlices.AddRange(slices);
        }

        // Make History
        FoldMove move = new()
        {
            stateBefore = currentPaperSlices,
            stateAfter = newSlices,
            rotatedObjects = toRotateSlices,
            foldData = foldIndex
        };
        movesHistory.Add(move);

        // Disable Previous Objects
        foreach (var obj in currentPaperSlices)
        {
            obj.SetActive(false);
        }
        foreach (var obj in newSlices)
        {
            obj.transform.parent = PaperObject.transform;
        }
        currentPaperSlices = newSlices;

        // Rotate
        RotateHandler.transform.SetPositionAndRotation(LinePointToGlobal(fold.line.GetPoint(.5f)), Quaternion.identity);

        foreach (var obj in toRotateSlices)
        {
            obj.transform.parent = RotateHandler.transform;
        }
        StartCoroutine(FoldAroundAxis(RotateHandler.transform, fold.line.GetAxis(), fold.angle, RotateHandler.transform.position - new Vector3(0, 0, 1 * 0.001f + 0.001f), .5f, false));

        // Move Fold Lines to new Mesh Coordinates
    }

    private bool CanSlice => folds.Count > 0;
    [Button]
    [EnableIf("CanSlice")]
    public void SliceMesh()
    {
        FoldPaper(0);
    }

    private Vector3 LinePointToGlobal(Vector3 point)
    {
        return PaperObject.transform.lossyScale.x * point + PaperObject.transform.position;
    }

    private Plane GetPlaneForObjectFromLine(Line line, GameObject other)
    {
        // Create a triangle between the tip and base so that we can get the normal
        Vector3 side1 = line.GetPoint(1f) - line.GetPoint(0f);

        //Get the point perpendicular to the triangle above which is the normal
        //https://docs.unity3d.com/Manual/ComputingNormalPerpendicularVector.html
        Vector3 normal = Vector3.Cross(side1, Vector3.forward).normalized;

        //Transform the normal so that it is aligned with the object we are slicing's transform.
        Vector3 transformedNormal = ((Vector3)(other.transform.localToWorldMatrix.transpose * normal)).normalized;

        //Get the enter position relative to the object we're cutting's local transform
        //Vector3 transformedStartingPoint = other.gameObject.transform.InverseTransformPoint(line.GetPoint(0f));

        Plane plane = new(transformedNormal, line.GetPoint(0f));

        var direction = Vector3.Dot(Vector3.up, transformedNormal);

        //Flip the plane so that we always know which side the positive mesh is on
        if (direction < 0)
        {
            plane.Flip();
        }

        return plane;
    }

    /*public void RotatePaper(bool back)
    {
        foreach (var c in corners)
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
    }*/

    IEnumerator FoldAroundAxis(Transform paperTransform, Vector3 axis, float angleChange, Vector3 endPosition, float duration, bool back)
    {
        clickeBlocked = true;

        Quaternion startRotation = paperTransform.rotation;
        Vector3 startPosition = paperTransform.position;

        float t = 0;
        while (t < duration)
        {
            paperTransform.rotation = startRotation * Quaternion.AngleAxis(angleChange * rotCurve.Evaluate(t / duration), axis);
            paperTransform.position = Vector3.Lerp(startPosition, endPosition, t / duration);

            t += Time.deltaTime;
            yield return null;
        }

        paperTransform.rotation = startRotation * Quaternion.AngleAxis(angleChange, axis);
        paperTransform.position = endPosition;

        /*if (back)
        {
            currentOrderIndex--;
        }
        else
        {
            currentOrderIndex++;
        }*/

        clickeBlocked = false;
    }

    public void SetUIOn()
    {
        LevelManager.PauseGame(false);
        UIObject.SetActive(true);
    }

    public void SetUIOff()
    {
        LevelManager.ResumeGame(false);
        UIObject.SetActive(false);
    }

    public bool IsUIOn()
    {
        return UIObject.activeSelf;
    }

    private void OnDrawGizmos()
    {
        Color color = Color.cyan;
        color.a = 125;
        /*Gizmos.color = color;
        Gizmos.DrawCube(PaperObject.transform.position, new Vector3(paperWidth * paperWidth * PaperObject.transform.lossyScale.x, paperHeight * paperHeight * PaperObject.transform.lossyScale.y, 0.001f));*/

        foreach (var obj in currentPaperSlices)
        {
            foreach (var fold in folds)
            {
                if (disabledFolds.Contains(folds.IndexOf(fold)))
                {
                    continue;
                }

                var line = fold.line;
                Gizmos.color = Color.red;
                Vector3 point1 = LinePointToGlobal(line.GetPoint(0f));
                Vector3 point2 = LinePointToGlobal(line.GetPoint(1f));
                Gizmos.DrawSphere(point1, .2f);
                Gizmos.DrawSphere(point2, .2f);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(point1, point2);

                // Plane (poprawiæ dla pionowych skalowanie itp)
                Vector3 normal = Vector3.Cross(line.GetAxis(), Vector3.forward).normalized;
                Quaternion rotation = Quaternion.LookRotation(obj.transform.TransformDirection(normal));
                Vector3 planePos = LinePointToGlobal(line.GetPoint(.5f));
                Matrix4x4 trs = Matrix4x4.TRS(planePos, rotation, Vector3.one);
                Gizmos.matrix = trs;
                color = Color.blue;
                color.a = 125;
                Gizmos.color = color;
                Vector3 lineAxis = point2 - point1;
                Gizmos.DrawCube(Vector3.zero, new Vector3(10f, 10f, 0.0001f));

                Gizmos.matrix = Matrix4x4.identity;
            }
        }
    }
}
