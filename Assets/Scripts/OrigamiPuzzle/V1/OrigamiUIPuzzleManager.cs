using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum DragPointXPos { Left = -1, Center = 0, Right = 1 }
public enum DragPointYPos { Top = 1, Center = 0, Bottom = -1 }

[System.Serializable]
public struct DragPoint
{
    public DragPointXPos xPos;
    public DragPointYPos yPos;
    public GameObject pointObj;
    [DisableIf("true")] public Vector3 startPos;
    [DisableIf("true")] public Vector3 offset;
}

[System.Serializable]
public struct OrigamiPuzzleObj
{
    [DisableIf("true")] public Mesh mesh;
    public Material material;
    public MeshFilter filter;
    public MeshRenderer renderer;

    public void UpdatePuzzle()
    {
        filter.mesh = mesh;
        renderer.material = material;
    }
}

public class OrigamiUIPuzzleManager : MonoBehaviour
{
    [SerializeField] private Camera UICamera;
    [SerializeField] private GameObject UIObject;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private OrigamiPuzzleObj frontPuzzle;
    [SerializeField] private OrigamiPuzzleObj backPuzzle;

    [SerializeField] private DragPoint dragPoint;

    private bool dragging = false;

    private void Start()
    {
        frontPuzzle.mesh = OrigamiPuzzleMeshGenerator.GeneratePaperMesh(width, height);
        frontPuzzle.mesh.name = "Front Origami Puzzle Mesh";
        frontPuzzle.UpdatePuzzle();

        backPuzzle.mesh = OrigamiPuzzleMeshGenerator.GeneratePaperMesh(width, height);
        backPuzzle.mesh.name = "Back Origami Puzzle Mesh";
        backPuzzle.UpdatePuzzle();
    }

    private void Update()
    {
        if (Physics.Raycast(UICamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000, LayerMask.GetMask("UI")))
        {
            if (hit.collider.gameObject == dragPoint.pointObj && Input.GetMouseButton(0))
            {
                dragging = true;
            }
        }
        
        if (dragging)
        {
            Vector3 mouseAxis = new(Input.GetAxis("Mouse X"), 0/*Input.GetAxis("Mouse Y") * transform.localScale.y*/, 0);
            dragPoint.offset += new Vector3(mouseAxis.x * transform.localScale.x, mouseAxis.y * transform.localScale.x, 0);
            if (Mathf.Abs(dragPoint.offset.x) >= 1 * transform.localScale.x)
            {
                mouseAxis.x = 0;
                if (dragPoint.offset.x < 0)
                {
                    dragPoint.offset.x -= 1 * transform.localScale.x + dragPoint.offset.x;
                }
                else
                {
                    dragPoint.offset.x += 1 * transform.localScale.x - dragPoint.offset.x;
                }
            }
            dragPoint.pointObj.transform.localPosition = dragPoint.startPos + dragPoint.offset;
            if (dragPoint.xPos == DragPointXPos.Right)
            {
                Vector3[] frontVerts = frontPuzzle.mesh.vertices;
                Vector3[] backVerts = backPuzzle.mesh.vertices;
                for (int h = 0; h < 21; h++)
                {
                    frontVerts[21 * h + 20] += new Vector3(mouseAxis.x, 0, 0);
                    backVerts[21 * h + 20] += new Vector3(mouseAxis.x, 0, 0);
                }
                frontPuzzle.mesh.vertices = frontVerts;
                backPuzzle.mesh.vertices = backVerts;
            }
            if (!Input.GetMouseButton(0))
            {
                dragging = false;
            }
        }
    }

    void SetDragPointPos(DragPoint drag)
    {
        Vector3 pointPos = new((float)dragPoint.xPos * transform.localScale.x * (width / 2f), (float)dragPoint.yPos * transform.localScale.y * (height / 2f), dragPoint.pointObj.transform.localPosition.z);
        dragPoint.startPos = pointPos + new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
        dragPoint.pointObj.transform.localPosition = dragPoint.startPos;
        dragPoint.offset = Vector3.zero;
    }

    List<Vector3> GetDragPointXPos(DragPoint drag, Vector3[] meshVerticies)
    {
        List<Vector3> verticies = new();

        // LEFT
        if (drag.xPos == DragPointXPos.Left)
        {
            
        }
        // CENTER
        else if (drag.xPos == DragPointXPos.Center)
        {

        }
        // RIGHT
        else
        {

        }
        return verticies;
    }

    List<Vector3> GetDragPointYPos(DragPoint drag, Vector3[] meshVerticies)
    {
        List<Vector3> verticies = new();

        return verticies;
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

    public void OnDrawGizmos()
    {
        if (frontPuzzle.mesh != null)
        {
            Vector3[] frontVerts = frontPuzzle.mesh.vertices;
            Gizmos.color = Color.red;
            for (int i = 0; i < 0; i++)
            {
                Gizmos.DrawSphere(frontVerts[i], .2f);
            }
        }
        if (backPuzzle.mesh != null)
        {
            Vector3[] backVerts = backPuzzle.mesh.vertices;
            Gizmos.color = Color.yellow;
            for (int i = 0; i < 0; i++)
            {
                Gizmos.DrawSphere(backVerts[i], .2f);
            }
        }
    }
}
