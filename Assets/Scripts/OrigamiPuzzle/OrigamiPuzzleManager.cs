using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class OrigamiPuzzleManager : MonoBehaviour
{
    // STRUCTS:
    [System.Serializable]
    public struct FoldMove
    {
        public GameObject[] oldState;
        public GameObject[] rotatedObjects;
        public Vector3 rotateHandlerPos;
        public Vector3 rotateAxis;
        public float rotateAngle;
        public FoldPaperClicker foldButton;
    }

    [System.Serializable]
    public struct FoldPaperBoundries
    {
        public Vector2 min;
        public Vector2 max;
    }

    [Header("Handlers:")]
    [SerializeField] private GameObject PaperObject = null;
    [SerializeField] private GameObject RotateHandler = null;
    [SerializeField] private GameObject LineHandler = null;
    [SerializeField] private GameObject SpriteHandler = null;

    [Header("Main Objects:")]
    [SerializeField] private List<GameObject> currentPaperSlices = new();
    [SerializeField] private FoldPaperClicker[] foldButtons;
    [SerializeField] private AnimationCurve rotCurve = new();

    [Header("Particle System:")]
    [SerializeField] private ParticleSystem confettiParticle;

    private bool True => true;
    [Foldout("info")] [DisableIf("True")] [SerializeField] private FoldPaperBoundries paperBoundries = new();
    [Foldout("info")] [DisableIf("True")] [SerializeField] private List<FoldMove> movesHistory = new();
    [Foldout("info")] [DisableIf("True")] [SerializeField] private bool solved = false;

    public FoldPaperBoundries boundries {
        get 
        {
            RecalculateFoldPaperBoundries();
            return paperBoundries; 
        } 
    }

    public int CurrentOrderIndex { get; private set; } = 0;
    private bool clickBlocked = false;

    private FoldPaperClicker highlightedFoldLineHandler;
    private bool resetHighlight = false;

    private void Start()
    {
        RecalculateFoldPaperBoundries();

        LineHandler.SetActive(false);

        SpriteHandler.SetActive(false);
    }

    private void Update()
    {
        if (LineHandler.activeSelf && resetHighlight)
        {
            HighlightFoldLine(highlightedFoldLineHandler, true);
        }
    }

    public void HighlightFoldLine(FoldPaperClicker clicker, bool enable)
    {
        var foldLine = clicker.fold.line;
        if (enable && (clicker != highlightedFoldLineHandler || resetHighlight))
        {
            // Enable Fold Line Highlight
            resetHighlight = false;
            highlightedFoldLineHandler = clicker;
            LineHandler.SetActive(true);
            var line = LineHandler.GetComponent<LineRenderer>();
            if (line.GetPosition(0) != PercentPointToGlobalPoint(foldLine.GetPoint(0f)) + new Vector3(0, 0, LineHandler.transform.localPosition.z))
            {
                line.SetPosition(0, PercentPointToGlobalPoint(foldLine.GetPoint(0f)) + new Vector3(0, 0, LineHandler.transform.localPosition.z));
            }
            if (line.GetPosition(1) != PercentPointToGlobalPoint(foldLine.GetPoint(1f)) + new Vector3(0, 0, LineHandler.transform.localPosition.z))
            {
                line.SetPosition(1, PercentPointToGlobalPoint(foldLine.GetPoint(1f)) + new Vector3(0, 0, LineHandler.transform.localPosition.z));
            }
        }
        else if (clicker == highlightedFoldLineHandler)
        {
            // Disable Fold Line Highlight
            LineHandler.SetActive(false);
            resetHighlight = true;
        }
    }

    public void FoldButtonClicked(FoldPaperClicker button)
    {
        if (!clickBlocked)
        {
            if (button.OrderIndex == -1)
            {
                FoldPaper(button);
            }
            else
            {
                var moveIndex = movesHistory.IndexOf(movesHistory.Find((m) => m.foldButton == button));
                if (moveIndex < CurrentOrderIndex - 1)
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

    IEnumerator FoldPaperBackToIndex(int index)
    {
        int tmpIndex = CurrentOrderIndex;
        while (CurrentOrderIndex != index)
        {
            if (tmpIndex == CurrentOrderIndex)
            {
                FoldPaperBack();
                tmpIndex--;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private void FoldPaper(FoldPaperClicker button)
    {
        button.OrderIndex = CurrentOrderIndex;
        List<GameObject> toRotateSlices = new();
        List<GameObject> newSlices = new();
        var fold = button.fold;
        foreach (var c in currentPaperSlices)
        {
            Plane p = GetPlaneForObjectFromLine(fold.line.GetPoint(0f), fold.line.GetPoint(1f), c);
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
            oldState = oldSlices.ToArray(),
            rotatedObjects = toRotateSlices.ToArray(),
            rotateHandlerPos = PercentPointToGlobalPoint(fold.line.GetPoint(.5f)),
            rotateAxis = (PercentPointToGlobalPoint(fold.line.GetPoint(1f)) - PercentPointToGlobalPoint(fold.line.GetPoint(0f))).normalized,
            rotateAngle = fold.angle,
            foldButton = button
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

            if (CheckIsSolved())
            {
                Solved();
            }

            resetHighlight = true;
        };

        StartCoroutine(FoldAroundAxis(RotateHandler.transform, (PercentPointToGlobalPoint(fold.line.GetPoint(1f)) - PercentPointToGlobalPoint(fold.line.GetPoint(0f))).normalized, fold.angle, RotateHandler.transform.position - new Vector3(0, 0, CurrentOrderIndex * 0.001f + 0.001f), .5f, false, finishAction));
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
            currentPaperSlices.Clear();
            currentPaperSlices.AddRange(move.oldState);

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

            move.foldButton.OrderIndex = -1;
            resetHighlight = true;
        };

        StartCoroutine(FoldAroundAxis(RotateHandler.transform, move.rotateAxis, -move.rotateAngle, RotateHandler.transform.position - new Vector3(0, 0, CurrentOrderIndex * 0.001f - 0.001f), .5f, true, finishAction));
        LineHandler.transform.position += new Vector3(0, 0, 0.001f);
    }

    public Vector3 PercentPointToGlobalPoint(Vector2 point)
    {
        float x = (point.x / 100f) * (boundries.max.x - boundries.min.x) + boundries.min.x;
        float y = (point.y / 100f) * (boundries.max.y - boundries.min.y) + boundries.min.y;
        return new Vector3(x, y, PaperObject.transform.position.z);
    }

    private Vector3 VertexPointToGlobal(Vector3 point, GameObject meshHandler)
    {
        Matrix4x4 trs = meshHandler.transform.localToWorldMatrix;
        return trs.MultiplyPoint3x4(point);
    }

    private Plane GetPlaneForObjectFromLine(Vector3 pointA, Vector3 pointB, GameObject other)
    {
        // Create a triangle between the tip and base so that we can get the normal
        Vector3 side1 = PercentPointToGlobalPoint(pointB) - PercentPointToGlobalPoint(pointA);

        //Get the point perpendicular to the triangle above which is the normal
        //https://docs.unity3d.com/Manual/ComputingNormalPerpendicularVector.html
        Vector3 normal = Vector3.Cross(side1, Vector3.forward).normalized;

        //Transform the normal so that it is aligned with the object we are slicing's transform.
        Vector3 transformedNormal = ((Vector3)(other.transform.localToWorldMatrix.transpose * normal)).normalized;

        //Get the enter position relative to the object we're cutting's local transform
        Vector3 transformedStartingPoint = other.transform.InverseTransformPoint(PercentPointToGlobalPoint(pointA));

        Plane plane = new(transformedNormal, transformedStartingPoint);

        return plane;
    }

    IEnumerator FoldAroundAxis(Transform paperTransform, Vector3 axis, float angleChange, Vector3 endPosition, float duration, bool back, Action onFinish)
    {
        clickBlocked = true;

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
            CurrentOrderIndex--;
        }
        else
        {
            CurrentOrderIndex++;
        }

        onFinish.Invoke();
        clickBlocked = false;
    }

    private bool CheckIsSolved()
    {
        if (movesHistory.Count != foldButtons.Length)
        {
            return false;
        }

        for (int i = 0; i < movesHistory.Count; i++)
        {
            var foldIndex = Array.IndexOf(foldButtons, movesHistory[i].foldButton);
            if (foldIndex != i)
            {
                return false;
            }
        }

        return true;
    }

    private void Solved()
    {
        GenerateSprite();
        ParticleSystem particle = Instantiate(confettiParticle, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
        particle.Play();
        foreach (var slice in currentPaperSlices)
        {
            slice.SetActive(false);
        }
        foreach (var button in foldButtons)
        {
            button.gameObject.SetActive(false);
        }
        SpriteHandler.SetActive(true);
        SpriteHandler.GetComponent<IPuzzleSolvedEvent>().Solved();
    }

    private void RecalculateFoldPaperBoundries() {
        paperBoundries.min.x = float.PositiveInfinity;
        paperBoundries.min.y = float.PositiveInfinity;
        paperBoundries.max.x = float.NegativeInfinity;
        paperBoundries.max.y = float.NegativeInfinity;

        foreach (var slice in currentPaperSlices)
        {
            Mesh sliceMesh;
#if UNITY_EDITOR
            //Only do this in the editor
            MeshFilter mf = slice.GetComponent<MeshFilter>();
            Mesh meshCopy = Instantiate<Mesh>(mf.sharedMesh) as Mesh;  //make a deep copy
            sliceMesh = meshCopy;
#else
            //Do this in play mode
            sliceMesh = slice.GetComponent<MeshFilter>().mesh;
#endif
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

        // Scale Sprite
        Vector2 spriteMin = Vector2.zero;
        Vector2 spriteMax = Vector2.zero;
        GetMaxAndMinCoordinates(sprite.vertices, ref spriteMin, ref spriteMax);
        spriteMin = VertexPointToGlobal(spriteMin, SpriteHandler);
        spriteMax = VertexPointToGlobal(spriteMax, SpriteHandler);

        float widthRatio = (paperBoundries.max.x - paperBoundries.min.x) / (spriteMax.x - spriteMin.x);
        float heightRatio = (paperBoundries.max.y - paperBoundries.min.y) / (spriteMax.y - spriteMin.y);

        SpriteHandler.transform.localScale = new Vector3(SpriteHandler.transform.localScale.x * widthRatio, SpriteHandler.transform.localScale.y * heightRatio, 1f);

        // Transform Sprite
        GetMaxAndMinCoordinates(sprite.vertices, ref spriteMin, ref spriteMax);
        spriteMin = VertexPointToGlobal(spriteMin, SpriteHandler);
        spriteMax = VertexPointToGlobal(spriteMax, SpriteHandler);

        SpriteHandler.transform.position += new Vector3(paperBoundries.max.x - spriteMax.x, paperBoundries.max.y - spriteMax.y, 0f);

        // Update Polygon Collider
        SpriteHandler.GetComponent<PolygonCollider2D>().pathCount = currentPaperSlices.Count;
        for (int i = 0; i < currentPaperSlices.Count; i++)
        {
            Mesh tempMesh = currentPaperSlices[i].GetComponent<MeshFilter>().mesh;
            List<Vector2> points = new();
            List<Vector2> simplifiedPoints = new();
            for (int v = 0; v < tempMesh.vertices.Length; v++)
            {
                Vector3 global = VertexPointToGlobal(tempMesh.vertices[v], currentPaperSlices[i]);
                Vector3 normalized = new((global.x - paperBoundries.min.x) / (paperBoundries.max.x - paperBoundries.min.x), (global.y - paperBoundries.min.y) / (paperBoundries.max.y - paperBoundries.min.y));
                Vector2 imageVert = new(normalized.x * (sprite.bounds.max.x - sprite.bounds.min.x) + sprite.bounds.min.x, normalized.y * (sprite.bounds.max.y - sprite.bounds.min.y) + sprite.bounds.min.y);
                points.Add(imageVert);
            }
            LineUtility.Simplify(points, 0.05f, simplifiedPoints);
            SpriteHandler.GetComponent<PolygonCollider2D>().SetPath(i, simplifiedPoints);
        }
    }

    private void GetMaxAndMinCoordinates(Vector2[] coordinates, ref Vector2 min, ref Vector2 max)
    {
        min.x = float.PositiveInfinity;
        min.y = float.PositiveInfinity;
        max.x = float.NegativeInfinity;
        max.y = float.NegativeInfinity;
        for (int i = 0; i < coordinates.Length; i++)
        {
            if (coordinates[i].x > max.x)
            {
                max.x = coordinates[i].x;
            }

            if (coordinates[i].y > max.y)
            {
                max.y = coordinates[i].y;
            }

            if (coordinates[i].x < min.x)
            {
                min.x = coordinates[i].x;
            }

            if (coordinates[i].y < min.y)
            {
                min.y = coordinates[i].y;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var button in foldButtons)
        {
            if (button.OrderIndex != -1)
            {
                continue;
            }

            button.OnDrawGizmosSelected();
            var line = button.fold.line;

            // Plane
            Vector3 side1 = PercentPointToGlobalPoint(line.GetPoint(1f)) - PercentPointToGlobalPoint(line.GetPoint(0f));
            Vector3 normal = Vector3.Cross(side1, Vector3.forward).normalized;
            Quaternion rotation = Quaternion.LookRotation(PaperObject.transform.TransformDirection(normal));
            Vector3 planePos = PercentPointToGlobalPoint(line.GetPoint(.5f));
            Matrix4x4 trs = Matrix4x4.TRS(planePos, rotation, new Vector3(2f, 10f, 0.0001f));
            Gizmos.matrix = trs;
            Color color = Color.blue;
            color.a = 125;
            Gizmos.color = color;
            Gizmos.DrawCube(Vector3.zero, Vector3.one);

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
                Gizmos.DrawSphere(VertexPointToGlobal(vertices[i], slice), .3f);
            }
        }
    }
}
