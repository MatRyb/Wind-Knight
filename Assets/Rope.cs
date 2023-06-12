using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private LineRenderer lineRenderer; 
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private float ropeSegLength = 0.25f;
    private int segmentLength = 100;
    private float lineWidth = 0.1f;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    CapsuleCollider2D capsule;
    MeshCollider meshCol;

    void Start()
    {
        this.lineRenderer = GetComponent<LineRenderer>();
        Vector3 ropeStartPoint = startPoint.transform.position;

        for(int i=0; i < segmentLength; i++)
        {
            ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLength;
        }

        meshCol = GetComponent<MeshCollider>();

        if(meshCol == null)
        {
            meshCol = gameObject.AddComponent<MeshCollider>();
        }


        //capsule = this.gameObject.AddComponent<CapsuleCollider2D>();

        //Vector2 width = new Vector2(lineWidth / 2, 0);
        //capsule.size = width;

        //capsule.radius = lineWidth / 2;
        //capsule.direction = -capsule/;
        //capsule.center = Vector2.zero;
        /*MeshCollider meshCollider = this.gameObject.AddComponent<MeshCollider>();

        mesh = new Mesh();
        lineRenderer.BakeMesh(mesh, true);
        meshCollider.sharedMesh = mesh;*/
    }

    void Update()
    {
        DrawRope(); 
    }
    private void FixedUpdate()
    {
        Simulate();

        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh);
        meshCol.sharedMesh = mesh;

        /*capsule.size = new Vector2(lineWidth, (endPoint.position - startPoint.position).magnitude);
        capsule.transform.position = startPoint.position + (endPoint.position - startPoint.position) / 2;
        capsule.transform.LookAt(startPoint.position);*/
        //capsule.height = (endPoint.position - startPoint.position).magnitude;
    }

    private void GenMesh()
    {
    }

    private void Simulate()
    {
        //Symulation
        Vector2 forceOfGravity = new Vector2(0f, -1.5f);
        for(int i=0; i<segmentLength; i++)
        {
            RopeSegment firstSegment = ropeSegments[i];
            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceOfGravity * Time.fixedDeltaTime;
            ropeSegments[i] = firstSegment;
        }

        //Constraints
        for(int i = 0; i < 50; i++)
        {
            ApplyConstraints();
        }
    }

    private void ApplyConstraints()
    {
        //1st constraint
        RopeSegment firstSegment = ropeSegments[0];
        firstSegment.posNow = startPoint.position;
        ropeSegments[0] = firstSegment;

        //second constraint
        RopeSegment endSegment = ropeSegments[segmentLength - 1];
        endSegment.posNow = endPoint.position;
        ropeSegments[segmentLength - 1] = endSegment;


        for (int i = 0; i < segmentLength - 1; i++)
        {
            RopeSegment firstSeg = ropeSegments[i];
            RopeSegment secondSeg = ropeSegments[i + 1];

            float distance = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(distance - ropeSegLength);
            Vector2 changeDirection = Vector2.zero;

            if (distance > ropeSegLength)
            {
                changeDirection = (firstSeg.posNow - secondSeg.posNow).normalized;
            }
            else if (distance < ropeSegLength)
            {
                changeDirection = (secondSeg.posNow - firstSeg.posNow).normalized;
            }

            Vector2 changeAmount = changeDirection * error;
            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.5f;
                ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
                ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                ropeSegments[i + 1] = secondSeg;
            }

        }
    }

    private void DrawRope()
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[this.segmentLength];
        for(int i=0; i<this.segmentLength; i++)
        {
            ropePositions[i] = this.ropeSegments[i].posNow; 
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions); 
    }

    public struct RopeSegment
    {
        public Vector2 posNow;  
        public Vector2 posOld;

        public RopeSegment(Vector3 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }
    }
}
