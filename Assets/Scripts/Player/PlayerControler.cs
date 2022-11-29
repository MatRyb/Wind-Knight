using UnityEngine;
using NaughtyAttributes;

public enum PlayerState { FALLING, MOVING }

public class PlayerControler : ObjectHealth
{
    public PlayerState playerState { get; private set; }

    public Transform playerBodyTransform = null;
    [SerializeField] private float minForceRadius = 1f;
    [SerializeField] private float maxForceRadius = 10f;
    [SerializeField] private float basePower = 2f;

    [Header("Rigidbody: ")]
    public Rigidbody2D playerRigidbody = null;
    [SerializeField] private float gravityScale = .2f;
    [SerializeField] private float mass = 10f;

    [Header("Mouse: ")]
    [SerializeField] private bool staticMousePos = false;
    [SerializeField] private GameObject mouseObject;
    [SerializeField] private float mouseSensitivity = 1f;


    [Foldout("info")]
    [DisableIf("true")] [SerializeField] private Vector2 virtualMousePosition;
    [Foldout("info")]
    [DisableIf("true")] public Vector2 velocity = Vector2.zero;
    [Foldout("info")]
    [DisableIf("true")] [SerializeField] private Vector2 positionChange = Vector2.zero;
    private Vector2 lastPosition;

    private void OnValidate()
    {
        if (playerBodyTransform == null)
        {
            playerBodyTransform = this.transform;
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

    void Update()
    {
        if (playerBodyTransform == null || mouseObject == null || playerRigidbody == null)
            return;

        positionChange = (new Vector2(transform.position.x, transform.position.y) - lastPosition);

        VirtualMousePositionCalculations();

        MouseVisualisation(playerBodyTransform.position);

        MovementBasis(playerBodyTransform.position);

        lastPosition = transform.position;

        //to test feather falling
        //playerRigidbody.mass = mass;
    }

    public override void OnDead()
    {
        LevelManager.InitRespawn();
        Debug.Log("Player Dead");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            OnDead();
        }
    }

    void VirtualMousePositionCalculations()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (staticMousePos)
            virtualMousePosition += positionChange;

        if (!(mouseDelta == Vector2.zero && velocity == Vector2.zero))
        {
            virtualMousePosition += mouseDelta * mouseSensitivity * GameTimer.timeMultiplayer;
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

        playerRigidbody.velocity = playerRigidbody.velocity * GameTimer.timeMultiplayer;
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
