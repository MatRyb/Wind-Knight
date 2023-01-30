using UnityEngine;
using NaughtyAttributes;

public enum PlayerState { FALLING, MOVING }

public class PlayerControler : ObjectHealth
{
    public PlayerState playerState { get; private set; }

    [SerializeField] private SpriteRenderer bodySprite;
    [SerializeField] private ParticleSystem deathParticle;
    private Color damageColor = new Color(1, 79/255, 79/255);
    private Color normalColor = new Color(1, 1, 1);

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
    [SerializeField] private Vector2 mouseBoundryOffset = Vector2.zero;


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

        this.StartHealth();
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

    public override void TakeDamage(float value)
    {
        LeanTween.value(bodySprite.gameObject, setSpriteColor, bodySprite.color, damageColor, 0.15f).setOnComplete(() => {
            LeanTween.value(bodySprite.gameObject, setSpriteColor, bodySprite.color, normalColor, 0.15f);
        });
        base.TakeDamage(value);
    }

    public void setSpriteColor(Color val)
    {
        bodySprite.color = val;
    }

    public override void OnDead()
    {
        ParticleSystem particle = Instantiate(deathParticle, this.gameObject.transform.position, new Quaternion(0, 0, 0, 0));
        particle.Play();
        Destroy(particle, 3);
        LevelManager.InitRespawn();
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
        if (GameTimer.timeMultiplayer == 0f)
            return;

        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (staticMousePos)
            virtualMousePosition += positionChange;

        if (!(mouseDelta == Vector2.zero && velocity == Vector2.zero))
        {
            virtualMousePosition += mouseDelta * mouseSensitivity;
            mouseObject.transform.position = virtualMousePosition;

            playerState = PlayerState.MOVING;
        }
        else
        {
            virtualMousePosition = (Vector2) mouseObject.transform.position + ((Vector2) mouseObject.transform.position - virtualMousePosition);

            playerState = PlayerState.FALLING;
        }
    }

    void BoundMousePositionToMainCameraView()
    {
        Vector3 finalMousePosition = virtualMousePosition;

        Vector3 lowerBound = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 upperBound = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        if (virtualMousePosition.x <= lowerBound.x)
        {
            finalMousePosition = new Vector3(lowerBound.x + mouseBoundryOffset.x, finalMousePosition.y, finalMousePosition.z);
        }
        else if (virtualMousePosition.x >= upperBound.x)
        {
            finalMousePosition = new Vector3(upperBound.x - mouseBoundryOffset.x, finalMousePosition.y, finalMousePosition.z);
        }

        if (virtualMousePosition.y <= lowerBound.y)
        {
            finalMousePosition = new Vector3(finalMousePosition.x, lowerBound.y + mouseBoundryOffset.y, finalMousePosition.z);
        }
        else if (virtualMousePosition.y >= upperBound.y)
        {
            finalMousePosition = new Vector3(finalMousePosition.x, upperBound.y - mouseBoundryOffset.y, finalMousePosition.z);
        }

        mouseObject.transform.position = finalMousePosition;
    }

    void MovementBasis(Vector2 playerPos)
    {
        if (GameTimer.timeMultiplayer == 0f)
        {
            playerRigidbody.bodyType = RigidbodyType2D.Static;
            return;
        }
        else
        {
            playerRigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

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

        playerRigidbody.velocity = playerRigidbody.velocity;
    }

    void MouseVisualisation(Vector2 playerPos)
    {
        float degreeToAdd = AdvancedMath.GetAngleBetweenPoints(playerPos, virtualMousePosition, Vector2.right + playerPos);

        mouseObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90 + degreeToAdd));

        BoundMousePositionToMainCameraView();
    }

    private void OnDrawGizmosSelected()
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

        Vector3 lowerBound = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 upperBound = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(lowerBound, 2f);
        Gizmos.DrawWireSphere(upperBound, 2f);
    }
}
