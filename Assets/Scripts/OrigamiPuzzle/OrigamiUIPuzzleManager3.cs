using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class OrigamiUIPuzzleManager3 : MonoBehaviour
{
    // STRUCTS:

    [System.Serializable]
    public struct FoldLine
    {
        [SerializeField] private Vector2 pointA;
        [SerializeField] private Vector2 pointB;

        public Vector3 GetPoint(float t)
        {
            return pointA + (pointB - pointA) * t;
        }
    }

    [System.Serializable]
    public struct FoldData
    {
        public FoldLine line;
        public float angle;
    }

    [System.Serializable]
    public struct FoldMove
    {
        public List<GameObject> oldState;
        public List<GameObject> rotatedObjects;
        public Vector3 rotateHandlerPos;
        public Vector3 rotateAxis;
        public float rotateAngle;
        public int foldIndex;
    }

    [System.Serializable]
    public struct FoldPaperBoundries
    {
        public Vector2 min;
        public Vector2 max;
    }

    [Header("UI Objects:")]
    [SerializeField] private Camera UICamera = null;
    [SerializeField] private GameObject UIObject = null;
    [SerializeField] private GameObject PaperObject = null;
    [SerializeField] private GameObject RotateHandler = null;
    [SerializeField] private GameObject LineHandler = null;
    [SerializeField] private GameObject SpriteHandler = null;

    [Header("Main Objects:")]
    [SerializeField] private List<GameObject> currentPaperSlices = new();
    [SerializeField] private List<FoldData> folds = new();
    [SerializeField] private AnimationCurve rotCurve = new();

    [Foldout("info")] [DisableIf("true")] [SerializeField] private FoldPaperBoundries paperBoundries = new();
    [Foldout("info")] [DisableIf("true")] [SerializeField] private List<FoldMove> movesHistory = new();
    [Foldout("info")] [DisableIf("true")] [SerializeField] private bool solved = false;

    private int currentOrderIndex = 0;
    private bool clickeBlocked = false;

    private void Start()
    {
        RecalculateFoldPaperBoundries();

        LineHandler.SetActive(false);

        GenerateSprite();
    }

    bool once = false;
    void Update()
    {
        if (solved)
        {
            if (once)
                return;
            GenerateSprite();
            SpriteHandler.AddComponent<Rigidbody2D>();
            once = true;
            return;
        }

        if (Physics.Raycast(UICamera.ScreenToWorldPoint(Input.mousePosition), UICamera.transform.forward, out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent(out FoldPaperClicker clicker))
            {
                int foldIndex = clicker.foldIndex;
                bool moveExists = movesHistory.Exists((m) => m.foldIndex == foldIndex);

                if (!moveExists)
                {
                    // Highlight Fold Line
                    LineHandler.SetActive(true);
                    var line = LineHandler.GetComponent<LineRenderer>();
                    line.SetPosition(0, PercentPointToGlobalPoint(folds[foldIndex].line.GetPoint(0f)) + new Vector3(0, 0, LineHandler.transform.localPosition.z));
                    line.SetPosition(1, PercentPointToGlobalPoint(folds[foldIndex].line.GetPoint(1f)) + new Vector3(0, 0, LineHandler.transform.localPosition.z));
                }

                if (Input.GetMouseButtonDown(0) && !clickeBlocked)
                {
                    // Disable Fold Line Highlight
                    LineHandler.SetActive(false);

                    if (!moveExists)
                    {
                        FoldPaper(foldIndex);
                    }
                    else
                    {
                        var moveIndex = movesHistory.IndexOf(movesHistory.Find((m) => m.foldIndex == foldIndex));
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
            else
            {
                // Disable Fold Line Highlight
                LineHandler.SetActive(false);
            }
        }
        else
        {
            // Disable Fold Line Highlight
            LineHandler.SetActive(false);
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

        // Disable Previous Objects
        foreach (var obj in currentPaperSlices)
        {
            obj.SetActive(false);
        }
        foreach (var obj in newSlices)
        {
            obj.transform.parent = PaperObject.transform;
        }
        List<GameObject> oldSlices = currentPaperSlices;
        currentPaperSlices = newSlices;

        // Rotate
        RotateHandler.transform.SetPositionAndRotation(PercentPointToGlobalPoint(fold.line.GetPoint(.5f)), Quaternion.identity);
        foreach (var obj in toRotateSlices)
        {
            obj.transform.parent = RotateHandler.transform;
        }

        // Make History
        FoldMove move = new()
        {
            oldState = oldSlices,
            rotatedObjects = toRotateSlices,
            rotateHandlerPos = PercentPointToGlobalPoint(fold.line.GetPoint(.5f)),
            rotateAxis = (PercentPointToGlobalPoint(fold.line.GetPoint(1f)) - PercentPointToGlobalPoint(fold.line.GetPoint(0f))).normalized,
            rotateAngle = fold.angle,
            foldIndex = foldIndex
        };
        movesHistory.Add(move);

        Action finishAction = () =>
        {
            // Reset RotateHandler
            var rotateTransforms = RotateHandler.GetComponentsInChildren<Transform>();
            foreach (var obj in rotateTransforms)
            {
                obj.parent = PaperObject.transform;
            }
            RotateHandler.transform.rotation = Quaternion.identity;

            // Move Fold Lines to new Mesh Coordinates
            RecalculateFoldPaperBoundries();

            solved = CheckIsSolved();
        };

        StartCoroutine(FoldAroundAxis(RotateHandler.transform, (PercentPointToGlobalPoint(fold.line.GetPoint(1f)) - PercentPointToGlobalPoint(fold.line.GetPoint(0f))).normalized, fold.angle, RotateHandler.transform.position - new Vector3(0, 0, currentOrderIndex * 0.001f + 0.001f), .5f, false, finishAction));
        LineHandler.transform.position += new Vector3(0, 0, -0.001f);
    }

    private void FoldPaperBack()
    {
        // Read History
        FoldMove move = movesHistory[^1];

        // Rotate
        RotateHandler.transform.SetPositionAndRotation(move.rotateHandlerPos, Quaternion.identity);
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

            movesHistory.Remove(move);

            // Clear RotateHandler Before Rotate
            var rotateTransforms = RotateHandler.GetComponentsInChildren<Transform>();
            foreach (var obj in rotateTransforms)
            {
                obj.parent = PaperObject.transform;
            }
            RotateHandler.transform.rotation = Quaternion.identity;

            // Move Fold Lines to new Mesh Coordinates
            RecalculateFoldPaperBoundries();
        };

        StartCoroutine(FoldAroundAxis(RotateHandler.transform, move.rotateAxis, -move.rotateAngle, RotateHandler.transform.position - new Vector3(0, 0, currentOrderIndex * 0.001f - 0.001f), .5f, true, finishAction));
        LineHandler.transform.position += new Vector3(0, 0, 0.001f);
    }

    private Vector3 PercentPointToGlobalPoint(Vector2 point)
    {
        float x = (point.x / 100f) * (paperBoundries.max.x - paperBoundries.min.x) + paperBoundries.min.x;
        float y = (point.y / 100f) * (paperBoundries.max.y - paperBoundries.min.y) + paperBoundries.min.y;
        return new Vector3(x, y, PaperObject.transform.position.z);
    }

    private Vector3 VertexPointToGlobal(Vector3 point, GameObject meshHandler)
    {
        Matrix4x4 trs = meshHandler.transform.localToWorldMatrix;
        return trs.MultiplyPoint3x4(point);
    }

    private Plane GetPlaneForObjectFromLine(FoldLine line, GameObject other)
    {
        // Create a triangle between the tip and base so that we can get the normal
        Vector3 side1 = PercentPointToGlobalPoint(line.GetPoint(1f)) - PercentPointToGlobalPoint(line.GetPoint(0f));

        //Get the point perpendicular to the triangle above which is the normal
        //https://docs.unity3d.com/Manual/ComputingNormalPerpendicularVector.html
        Vector3 normal = Vector3.Cross(side1, Vector3.forward).normalized;

        //Transform the normal so that it is aligned with the object we are slicing's transform.
        Vector3 transformedNormal = ((Vector3)(other.transform.localToWorldMatrix.transpose * normal)).normalized;

        //Get the enter position relative to the object we're cutting's local transform
        Vector3 transformedStartingPoint = other.transform.InverseTransformPoint(PercentPointToGlobalPoint(line.GetPoint(0f)));

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

    private bool CheckIsSolved()
    {
        if (movesHistory.Count != folds.Count)
        {
            return false;
        }

        for (int i = 0; i < movesHistory.Count; i++)
        {
            if (movesHistory[i].foldIndex != i)
            {
                return false;
            }
        }

        return true;
    }

    private void RecalculateFoldPaperBoundries() {
        paperBoundries.min.x = float.PositiveInfinity;
        paperBoundries.min.y = float.PositiveInfinity;
        paperBoundries.max.x = float.NegativeInfinity;
        paperBoundries.max.y = float.NegativeInfinity;

        foreach (var slice in currentPaperSlices)
        {
            Mesh sliceMesh = slice.GetComponent<MeshFilter>().mesh;
            for (int i = 0; i < sliceMesh.vertices.Length; i++)
            {
                Vector3 globalVertex = VertexPointToGlobal(sliceMesh.vertices[i], slice);
                if (globalVertex.x > paperBoundries.max.x)
                {
                    paperBoundries.max.x = globalVertex.x;
                }

                if (globalVertex.y > paperBoundries.max.y)
                {
                    paperBoundries.max.y = globalVertex.y;
                }

                if (globalVertex.x < paperBoundries.min.x)
                {
                    paperBoundries.min.x = globalVertex.x;
                }

                if (globalVertex.y < paperBoundries.min.y)
                {
                    paperBoundries.min.y = globalVertex.y;
                }
            }
        }
    }

    private void GenerateSprite()
    {
        SpriteRenderer renderer = SpriteHandler.GetComponent<SpriteRenderer>();
        Sprite sprite = renderer.sprite;
        List<Vector3> verticies = new();
        List<int> triangles = new();
        int vertexOffset = verticies.Count;

        for (int i = 0; i < currentPaperSlices.Count; i++)
        {
            Mesh tempMesh = currentPaperSlices[i].GetComponent<MeshFilter>().mesh;
            for (int v = 0; v < tempMesh.vertices.Length; v++)
            {
                Vector3 global = VertexPointToGlobal(tempMesh.vertices[v], currentPaperSlices[i]);
                Vector3 normalized = new((global.x - paperBoundries.min.x) / (paperBoundries.max.x - paperBoundries.min.x), (global.y - paperBoundries.min.y) / (paperBoundries.max.y - paperBoundries.min.y));
                verticies.Add(normalized);
            }
            for (int t = 0; t < tempMesh.triangles.Length; t++)
            {
                triangles.Add(tempMesh.triangles[t] + vertexOffset);
            }

            vertexOffset = verticies.Count;
        }

        MeshToSprite.ConvertMeshsToSprite(verticies.ToArray(), triangles.ToArray(), ref sprite);
        renderer.sprite = sprite;

        SpriteHandler.transform.localScale = new Vector3(1f, (paperBoundries.max.y - paperBoundries.min.y) / (paperBoundries.max.x - paperBoundries.min.x), 1f);

        // Update Polygon Collider (Przy kwadracie nie dzia�a XD)
        List<Vector2> points = new();
        List<Vector2> simplifiedPoints = new();
        points.AddRange(sprite.vertices);
        LineUtility.Simplify(points, 0.05f, simplifiedPoints);
        SpriteHandler.GetComponent<PolygonCollider2D>().SetPath(0, simplifiedPoints);
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

    private void OnDrawGizmosSelected()
    {
        foreach (var fold in folds)
        {
            if (movesHistory.Exists((m) => m.foldIndex == folds.IndexOf(fold)))
            {
                continue;
            }

            var line = fold.line;
            Gizmos.color = Color.red;
            Vector3 pointA = PercentPointToGlobalPoint(line.GetPoint(0f));
            Vector3 midPoint = PercentPointToGlobalPoint(line.GetPoint(.5f));
            Vector3 pointB = PercentPointToGlobalPoint(line.GetPoint(1f));
            Gizmos.DrawSphere(pointA, .2f);
            Gizmos.DrawSphere(pointB, .2f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA, pointB);
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(midPoint, .1f);

            // Plane
            Vector3 side1 = PercentPointToGlobalPoint(line.GetPoint(1f)) - PercentPointToGlobalPoint(line.GetPoint(0f));
            Vector3 normal = Vector3.Cross(side1, Vector3.forward).normalized;
            Quaternion rotation = Quaternion.LookRotation(PaperObject.transform.TransformDirection(normal));
            Vector3 planePos = PercentPointToGlobalPoint(line.GetPoint(.5f));
            Matrix4x4 trs = Matrix4x4.TRS(planePos, rotation, Vector3.one);
            Gizmos.matrix = trs;
            Color color = Color.blue;
            color.a = 125;
            Gizmos.color = color;
            Gizmos.DrawCube(Vector3.zero, new Vector3(2f, 10f, 0.0001f));

            Gizmos.matrix = Matrix4x4.identity;
        }

        foreach (var slice in currentPaperSlices)
        {
            Mesh mesh;
#if UNITY_EDITOR
            //Only do this in the editor
            MeshFilter mf = slice.GetComponent<MeshFilter>();
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
