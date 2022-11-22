using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum PlayerState { FALLING, MOVING }

public class PlayerControler : MonoBehaviour
{
    public PlayerState playerState { get; private set; }

    [SerializeField] public float hitPoints;

    [SerializeField] public Transform playerBodyTransform = null;
    [SerializeField] private float minForceRadius = 1f;
    [SerializeField] private float maxForceRadius = 20f;
    [SerializeField] private float basePower = 2f;

    [Header("Rigidbody: ")]
    [SerializeField] public Rigidbody2D playerRigidbody = null;
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private float mass = .1f;

    [Header("Mouse: ")]
    [SerializeField] private GameObject mouseObject;
    [SerializeField] private float mouseSensitivity = 1f;
       
    [Foldout("info")]
    [DisableIf("true")] [SerializeField] private Vector2 virtualMousePosition;
    [Foldout("info")]
    [DisableIf("true")] [SerializeField] public Vector2 velocity = Vector2.zero;

    private void OnValidate()
    {
        if (playerBodyTransform == null)
        {
            Debug.LogError("Player body can't be null. Please provide one. :)");
        }

        if (mouseObject == null)
        {
            Debug.LogError("Mouse Object can't be null. Please provide one. :)");
        }

        if (GetComponent<Rigidbody2D>() != null && playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
        }
        else if (playerRigidbody == null)
        {
            Debug.LogError("Players Rigidbody can't be null. Please provide one. :)");
        }
    }

    private void Awake()
    {
        virtualMousePosition = playerBodyTransform.position + Vector3.right * minForceRadius;
        velocity = Vector2.zero;

        if (mouseObject != null)
        {
            mouseObject.transform.position = virtualMousePosition;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = velocity;
            playerRigidbody.gravityScale = gravityScale;
            playerRigidbody.mass = mass;
        }
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (hitPoints <= 0.0f)
        {
            Debug.Log("You ded");
        }

        if (playerBodyTransform == null || mouseObject == null || playerRigidbody == null)
            return;

        VirtualMousePositionCalculations();

        MouseVisualisation(playerBodyTransform.position);

        MovementBasis(playerBodyTransform.position);

        //to test feather falling
        //playerRigidbody.mass = mass;
    }

    void VirtualMousePositionCalculations()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (!(mouseDelta == Vector2.zero && velocity == Vector2.zero))
        {
            virtualMousePosition += mouseDelta * mouseSensitivity;
            mouseObject.transform.position = virtualMousePosition;

            playerState = PlayerState.MOVING;
        }
        else
        {
            virtualMousePosition = mouseObject.transform.position;

            playerState = PlayerState.FALLING;
        }
    }

    void MovementBasis(Vector2 playerPos)
    {
        float totalDist = Mathf.Abs(Vector2.Distance(playerPos, virtualMousePosition));
        velocity = Vector2.zero;
        if (totalDist > minForceRadius)
        {
            playerRigidbody.gravityScale = 0f;
            Vector2 calculateTo = virtualMousePosition;
            if (totalDist > maxForceRadius)
            {
                calculateTo = (virtualMousePosition - playerPos).normalized * maxForceRadius + playerPos;
            }
            velocity = (calculateTo - playerPos) * basePower;
            playerRigidbody.velocity = velocity;
        }
        else
        {
            playerRigidbody.gravityScale = gravityScale;
        }
    }

    void MouseVisualisation(Vector2 playerPos)
    {
        float degreeToAdd = AdvancedMath.GetAngleBetweenPoints(playerPos, virtualMousePosition, Vector2.right + playerPos);

        mouseObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90 + degreeToAdd));
    }

    private void OnDrawGizmos()
    {
        Vector2 playerPos = playerBodyTransform.position;
        float totalDist = Mathf.Abs(Vector2.Distance(playerPos, virtualMousePosition));

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerPos, minForceRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerPos, virtualMousePosition);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(playerPos, totalDist);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(playerPos, maxForceRadius);

        if (totalDist > maxForceRadius)
        {
            Vector2 calculateTo = (virtualMousePosition - playerPos).normalized * maxForceRadius + playerPos;
            Gizmos.color = Color.white;
            Gizmos.DrawLine(playerPos, calculateTo);
        }
    }
}
