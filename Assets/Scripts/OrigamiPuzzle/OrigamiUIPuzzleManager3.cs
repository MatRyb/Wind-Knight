using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

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
    public List<GameObject> oldState;
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

    public AnimationCurve rotCurve = new();

    public bool clickeBlocked = false;

    private int currentOrderIndex = 0;

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
                    if (!movesHistory.Exists((m) => m.foldData == foldIndex))
                    {
                        FoldPaper(foldIndex);
                    }
                    else
                    {
                        var moveIndex = movesHistory.IndexOf(movesHistory.Find((m) => m.foldData == foldIndex));
                        if (moveIndex < currentOrderIndex - 1)
                        {
                            StartCoroutine(FoldPaperBackToIndex(moveIndex));
                        }
                        else
                        {
                            FoldPaperBack();
                        }
                    }
                }
            }
        }
    }

    IEnumerator FoldPaperBackToIndex(int index)
    {
        int tmpIndex = currentOrderIndex;
        while (currentOrderIndex != index)
        {
            if (tmpIndex == currentOrderIndex)
            {
                FoldPaperBack();
                tmpIndex--;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private void FoldPaper(int foldIndex)
    {
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
            oldState = currentPaperSlices,
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

        Action finishAction = () =>
        {
            // Clear RotateHandler Before Rotate
            var rotateTransforms = RotateHandler.GetComponentsInChildren<Transform>();
            foreach (var obj in rotateTransforms)
            {
                obj.parent = PaperObject.transform;
            }
        };

        StartCoroutine(FoldAroundAxis(RotateHandler.transform, fold.line.GetAxis(), fold.angle, RotateHandler.transform.position - new Vector3(0, 0, currentOrderIndex * 0.001f + 0.001f), .5f, false, finishAction));

        // Move Fold Lines to new Mesh Coordinates
    }

    private void FoldPaperBack()
    {
        // Read History
        FoldMove move = movesHistory[^1];
        FoldData fold = folds[move.foldData];
        movesHistory.Remove(move);

        // Rotate
        RotateHandler.transform.SetPositionAndRotation(LinePointToGlobal(fold.line.GetPoint(.5f)), Quaternion.identity);
        foreach (var obj in move.rotatedObjects)
        {
            obj.transform.parent = RotateHandler.transform;
        }

        Action finishAction = () =>
        {
            // Destrony Slices and Enable Old Objects
            foreach (var obj in currentPaperSlices)
            {
                Destroy(obj);
            }
            foreach (var obj in move.oldState)
            {
                obj.SetActive(true);
            }
            currentPaperSlices = move.oldState;

            // Clear RotateHandler Before Rotate
            var rotateTransforms = RotateHandler.GetComponentsInChildren<Transform>();
            foreach (var obj in rotateTransforms)
            {
                obj.parent = PaperObject.transform;
            }

            // Move Fold Lines to new Mesh Coordinates
        };

        StartCoroutine(FoldAroundAxis(RotateHandler.transform, fold.line.GetAxis(), -fold.angle, RotateHandler.transform.position - new Vector3(0, 0, currentOrderIndex * 0.001f - 0.001f), .5f, true, finishAction));
    }

    private bool CanSlice => folds.Count > 0;
    private bool clicked = false;
    [Button]
    [EnableIf("CanSlice")]
    public void SliceMesh()
    {
        if (clicked)
        {
            FoldPaper(0);
        }
        else
        {
            FoldPaperBack();
        }
        clicked = !clicked;
    }

    private Vector3 LinePointToGlobal(Vector3 point)
    {
        Matrix4x4 trs = PaperObject.transform.localToWorldMatrix;
        return trs.MultiplyPoint3x4(point);
    }

    private Vector3 VertexPointToGlobal(Vector3 point, GameObject meshHandler)
    {
        Matrix4x4 trs = meshHandler.transform.localToWorldMatrix;
        return trs.MultiplyPoint3x4(point);
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
        Vector3 transformedStartingPoint = other.transform.InverseTransformPoint(LinePointToGlobal(line.GetPoint(0f)));

        Plane plane = new(transformedNormal, transformedStartingPoint);

        /*var direction = Vector3.Dot(Vector3.up, normal);

        //Flip the plane so that we always know which side the positive mesh is on
        if (direction < 0)
        {
            plane.Flip();
        }*/

        return plane;
    }

    IEnumerator FoldAroundAxis(Transform paperTransform, Vector3 axis, float angleChange, Vector3 endPosition, float duration, bool back, Action onFinish)
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

        paperTransform.SetPositionAndRotation(endPosition, startRotation * Quaternion.AngleAxis(angleChange, axis));

        if (back)
        {
            currentOrderIndex--;
        }
        else
        {
            currentOrderIndex++;
        }

        onFinish.Invoke();
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
        /*Color color = Color.cyan;
        color.a = 125;
        Gizmos.color = color;
        Gizmos.DrawCube(PaperObject.transform.position, new Vector3(paperWidth * paperWidth * PaperObject.transform.lossyScale.x, paperHeight * paperHeight * PaperObject.transform.lossyScale.y, 0.001f));*/

        foreach (var fold in folds)
        {
            if (movesHistory.Exists((m) => m.foldData == folds.IndexOf(fold)))
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

            // Plane
            Vector3 normal = Vector3.Cross(line.GetAxis(), Vector3.forward).normalized;
            Quaternion rotation = Quaternion.LookRotation(PaperObject.transform.TransformDirection(-normal));
            Vector3 planePos = LinePointToGlobal(line.GetPoint(.5f));
            Matrix4x4 trs = Matrix4x4.TRS(planePos, rotation, Vector3.one);
            Gizmos.matrix = trs;
            Color color = Color.blue;
            color.a = 125;
            Gizmos.color = color;
            Vector3 lineAxis = point2 - point1;
            Gizmos.DrawCube(Vector3.zero, new Vector3(2f, 10f, 0.0001f));

            Gizmos.matrix = Matrix4x4.identity;
        }

        foreach (var slice in currentPaperSlices)
        {
            Mesh mesh;
#if UNITY_EDITOR
            //Only do this in the editor
            MeshFilter mf = slice.GetComponent<MeshFilter>();   //a better way of getting the meshfilter using Generics
            Mesh meshCopy = Instantiate<Mesh>(mf.sharedMesh) as Mesh;  //make a deep copy
            mesh = meshCopy;
#else
            //Do this in play mode
            mesh = slice.GetComponent<MeshFilter>().mesh;
#endif
            Gizmos.color = Color.yellow;
            var vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(VertexPointToGlobal(vertices[i], slice), .1f);
            }
        }
    }
}
