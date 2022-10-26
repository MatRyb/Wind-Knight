using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PlayerControler : MonoBehaviour
{
    [SerializeField] private Transform playerBodyTransform = null;
    [SerializeField] private float minForceRadius = 0.1f;
    [SerializeField] private float maxForceRadius = 20f;
    [SerializeField] private float basePower = 2f;

    [Header("Mouse: ")]
    [SerializeField] private GameObject mouseObject;
    [SerializeField] private float mouseSensitivity = 2f;
       
    [Foldout("info")]
    [DisableIf("true")] [SerializeField] private Vector2 virtualMousePosition;
    [Foldout("info")]
    [DisableIf("true")] [SerializeField] private Vector2 velocity;

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
    }

    private void Awake()
    {
        virtualMousePosition = playerBodyTransform.position + Vector3.right * minForceRadius;
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
        if (playerBodyTransform == null || mouseObject == null)
            return;

        VirtualMousePositionCalculations();

        MouseVisualisation(playerBodyTransform.position);

        MovementBasis(playerBodyTransform.position);
    }

    void VirtualMousePositionCalculations()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        virtualMousePosition += mouseDelta * mouseSensitivity;
        mouseObject.transform.position = virtualMousePosition;
    }

    void MovementBasis(Vector2 playerPos)
    {
        float totalDist = Mathf.Abs(Vector2.Distance(playerPos, virtualMousePosition));
        velocity = Vector2.zero;
        if (totalDist > minForceRadius)
        {
            Vector2 calculateTo = virtualMousePosition;
            if (totalDist > maxForceRadius)
            {
                calculateTo = (virtualMousePosition - playerPos).normalized * maxForceRadius + playerPos;
            }
            velocity = (calculateTo - playerPos) * basePower;
            Vector2 totalForce = velocity * Time.deltaTime;
            playerBodyTransform.position += new Vector3(totalForce.x, totalForce.y, 0f);
        }
    }

    void MouseVisualisation(Vector2 playerPos)
    {
        float deltaX = virtualMousePosition.x - playerPos.x;
        float deltaY = virtualMousePosition.y - playerPos.y;
        float r = Vector2.Distance(playerPos, virtualMousePosition);
        float degreeToAdd;
        if (deltaY > 0)
            degreeToAdd = Mathf.Acos(deltaX / r) * 180f / Mathf.PI;
        else
            degreeToAdd = -Mathf.Acos(deltaX / r) * 180f / Mathf.PI;

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
