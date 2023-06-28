using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public enum PlayerState { FALLING, MOVING }

public class PlayerControler : ObjectHealth
{
    public PlayerState playerState { get; private set; }

    [SerializeField] private ParticleSystem deathParticle;
    private Color damageColor = new(1, 79/255, 79/255);
    private Color normalColor = new(1, 1, 1);

    public Transform playerBodyTransform = null;
    [SerializeField] private float minForceRadius = 1f;
    [SerializeField] private float maxForceRadius = 10f;
    [SerializeField] private float basePower = 2f;

    [Header("Body: ")]
    [SerializeField] private GameObject body;
    [SerializeField] private bool m_FacingRight = true;
    [SerializeField] private float maxRotation = 90;
    [SerializeField] private float minRotation = -90;

    [Header("Rigidbody: ")]
    public Rigidbody2D playerRigidbody = null;
    [SerializeField] private float gravityScale = .2f;
    [SerializeField] private float mass = 10f;

    [Header("Mouse: ")]
    [SerializeField] private bool staticMousePos = false;
    [SerializeField] private GameObject mouseObject;
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private Vector2 mouseBoundryOffset = Vector2.zero;
    [SerializeField] private List<Sprite> mouseStates;

    [Header("Audio:")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip damageClip;

    [Foldout("info")]
    [DisableIf("true")] [SerializeField] private Vector2 virtualMousePosition;
    [Foldout("info")]
    [DisableIf("true")] public Vector2 velocity = Vector2.zero;
    [Foldout("info")]
    [DisableIf("true")] public float minSpeed = 0f;
    [Foldout("info")]
    [DisableIf("true")] public float speed = 0f;
    [Foldout("info")]
    [DisableIf("true")] public float maxSpeed = 0f;
    [Foldout("info")]
    [DisableIf("true")] [SerializeField] private Vector2 positionChange = Vector2.zero;
    private Vector2 lastPosition;
    [DisableIf("true")] public int objectHits = 0;
    [SerializeField] public int maxObjectHits = 100;

    private void OnValidate()
    {
        if (playerBodyTransform == null)
        {
            playerBodyTransform = transform;
        }

        if (body == null)
        {
            Debug.LogError(gameObject.name + " Body Object can't be null. Please provide one. :)");
        }

        if (mouseObject == null)
        {
            Debug.LogError(gameObject.name + " Mouse Object can't be null. Please provide one. :)");
        }

        if (mouseStates.Count == 0)
        {
            Debug.LogError(gameObject.name + " There must be at least 2 states.");
        }

        if (GetComponentInParent<Rigidbody2D>() != null && playerRigidbody == null)
        {
            playerRigidbody = GetComponentInParent<Rigidbody2D>();
        }
        else if (playerRigidbody == null)
        {
            Debug.LogError(gameObject.name + " Players Rigidbody can't be null. Please provide one. :)");
        }
    }

    private void Awake()
    {
        velocity = Vector2.zero;

        mouseInit();

        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = velocity;
            playerRigidbody.gravityScale = gravityScale;
            playerRigidbody.mass = mass;
        }

        StartHealth();

        objectHits = maxObjectHits;
    }

    public void mouseInit()
    {
        virtualMousePosition = playerBodyTransform.position + Vector3.right * minForceRadius;

        if (mouseObject != null)
        {
            mouseObject.transform.position = virtualMousePosition;
        }
    }

    void Update()
    {
        if (playerBodyTransform == null || mouseObject == null || playerRigidbody == null)
            return;

        positionChange = (new Vector2(transform.position.x, transform.position.y) - lastPosition);

        VirtualMousePositionCalculations();

        if (virtualMousePosition.x >= transform.position.x && !m_FacingRight)
        {
            Flip();
        }
        else if (virtualMousePosition.x < transform.position.x && m_FacingRight)
        {
            Flip();
        }

        float y = virtualMousePosition.y - transform.position.y;

        float x = Mathf.Abs(virtualMousePosition.x - transform.position.x);

        if (Mathf.Atan2(y, x) * 180 / Mathf.PI >= minRotation && Mathf.Atan2(y, x) * 180 / Mathf.PI <= maxRotation)
        {
            body.transform.localRotation = Quaternion.Euler(body.transform.rotation.x, body.transform.rotation.y, m_FacingRight ? Mathf.Atan2(y, x) * 180 / Mathf.PI : - Mathf.Atan2(y, x) * 180 / Mathf.PI);
        }
        else if (Mathf.Atan2(y, x) * 180 / Mathf.PI < minRotation)
        {
            body.transform.localRotation = Quaternion.Euler(body.transform.rotation.x, body.transform.rotation.y, m_FacingRight ? minRotation : maxRotation);
        }
        else if (Mathf.Atan2(y, x) * 180 / Mathf.PI > maxRotation)
        {
            body.transform.localRotation = Quaternion.Euler(body.transform.rotation.x, body.transform.rotation.y, m_FacingRight ? maxRotation : minRotation);
        }

        MouseVisualisation(playerBodyTransform.position);

        MovementBasis(playerBodyTransform.position);

        lastPosition = transform.position;

        //to test feather falling
        //playerRigidbody.mass = mass;
    }

    public override void TakeDamage(float value)
    {
        LeanTween.value(body.gameObject, SetSpriteColor, body.GetComponent<SpriteRenderer>().color, damageColor, 0.15f).setOnComplete(() => {
            LeanTween.value(body.gameObject, SetSpriteColor, body.GetComponent<SpriteRenderer>().color, normalColor, 0.15f);
        });
        source.clip = damageClip;
        source.Play();
        base.TakeDamage(value);
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = body.transform.localScale;
        theScale.x *= -1;
        body.transform.localScale = theScale;
    }

    public void SetSpriteColor(Color val)
    {
        body.GetComponent<SpriteRenderer>().color = val;
    }

    public override void OnDead()
    {
        ParticleSystem particle = Instantiate(deathParticle, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
        particle.Play();
        Destroy(particle.gameObject, 3);
        LevelManager.InitRespawn();
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            OnDead();
        }
    }*/

    void VirtualMousePositionCalculations()
    {
        if (GameTimer.TimeMultiplier == GameTimer.STOPPED)
            return;

        Vector2 mouseDelta = new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

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
            virtualMousePosition = mouseObject.transform.position;

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
        if (GameTimer.TimeMultiplier == GameTimer.STOPPED)
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
            int state = Mathf.CeilToInt(((mouseStates.Count-1) / maxForceRadius) * Mathf.Abs(Vector2.Distance(calculateTo, playerPos)));
            mouseObject.GetComponent<SpriteRenderer>().sprite = mouseStates[state == mouseStates.Count ? mouseStates.Count - 1 : state];
            speed = Mathf.Abs(Vector2.Distance(calculateTo, playerPos)) * basePower;
            velocity = (calculateTo - playerPos) * basePower;
            playerRigidbody.velocity = velocity;
        }
        else
        {
            mouseObject.GetComponent<SpriteRenderer>().sprite = mouseStates[0];
            playerRigidbody.gravityScale = gravityScale;
        }

        maxSpeed = Mathf.Abs(Vector2.Distance(((virtualMousePosition - playerPos).normalized * maxForceRadius) + playerPos, playerPos)) * basePower;

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
        if (!Application.isPlaying)
        {
            virtualMousePosition = playerBodyTransform.position + Vector3.right * minForceRadius;
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out ObjectScript _))
        {
            if (--objectHits <= 0)
            {
                OnDead();
            }
        }
    }
}
