using UnityEngine;

public class BetterWaveEnemy : EnemyController
{
    [SerializeField] private Rigidbody2D rb;
    private WaveGenerator wg;
    [SerializeField] private float escapeRange;
    [SerializeField] private float rangeOffset = 50f;
    [SerializeField] private float moveDist = 10f;

    [SerializeField] private LayerMask rayCastLayer;
    [SerializeField] private ParticleSystem portalOut;
    [SerializeField] private ParticleSystem portalIn;
    [SerializeField] private AudioClip teleportClip;

    private bool canAttack = true;
    private bool moved = false;

    private float factorX;
    private float factorY;

    void Awake()
    {
        if (rb == null && (rb = gameObject.GetComponentInParent<Rigidbody2D>()) != null)
        {
            rb.gravityScale = 0f;
        }
        else if (rb != null)
        {
            rb.gravityScale = 0f;
        }
        else if (rb == null)
        {
            Debug.LogError("Wave Enemy needs Rigidbody2D");
        }

        if (wg == null && !TryGetComponent(out wg))
        {
            Debug.LogError("Wave Enemy needs WaveGenerator");
        }

        if (FindObjectOfType<PlayerControler>() != null)
        {
            player = FindObjectOfType<PlayerControler>().transform;
        }
    }

    void Start()
    {
        StartHealth();
        rb.velocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        Vector3 dir = player.position - transform.parent.position;
        float distance = Mathf.Abs(Vector3.Distance(Vector3.zero, dir));

        if (IsObjectBlockedByOtherObject(player.gameObject, viewRayBlockingLayers) || distance > range + rangeOffset)
        {
            canAttack = true;
            return;
        } 
        else if (distance > escapeRange && distance <= range)
        {
            if (GameTimer.TimeMultiplier == GameTimer.PLAYING)
            {
                float degreeToAdd = AdvancedMath.GetAngleBetweenPoints(transform.parent.position, player.position, Vector3.right + transform.parent.position);

                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, degreeToAdd));

                if (canAttack)
                {
                    canAttack = false;
                    Attack();
                }
            }
        }
        else if (distance <= escapeRange)
        {
            if (GameTimer.TimeMultiplier == GameTimer.PLAYING && !moved)
            {
                float degree = AdvancedMath.GetAngleBetweenPoints(transform.parent.position, player.position, Vector3.right + transform.parent.position);

                factorX = -Mathf.Cos(degree * Mathf.PI / 180);

                factorY = -Mathf.Sin(degree * Mathf.PI / 180);

                moved = true;
                if (Physics2D.Raycast(transform.parent.position, new Vector2(factorX, factorY), moveDist, rayCastLayer))
                {
                    GameObject particleIn = Instantiate(portalIn.gameObject, transform.parent.position, transform.rotation);
                    particleIn.GetComponent<ParticleSystem>().Play();
                    AudioSource s = Instantiate(source, transform.parent.position, new Quaternion(0, 0, 0, 0)).GetComponent<AudioSource>();
                    s.clip = teleportClip;
                    s.volume = volume;
                    s.mute = mute;
                    s.Play();
                    Destroy(s.gameObject, 2f);
                    Destroy(particleIn.gameObject, 2f);
                    transform.parent.position += new Vector3(moveDist * -factorX, moveDist * -factorY, 0);
                    GameObject particleOut = Instantiate(portalOut.gameObject, transform.parent.position, transform.rotation);
                    particleOut.GetComponent<ParticleSystem>().Play();
                    Destroy(particleOut.gameObject, 2f);
                }
                else
                {
                    GameObject particleIn = Instantiate(portalIn.gameObject, transform.parent.position, transform.rotation);
                    particleIn.GetComponent<ParticleSystem>().Play();
                    AudioSource s = Instantiate(source, transform.parent.position, new Quaternion(0, 0, 0, 0)).GetComponent<AudioSource>();
                    s.clip = teleportClip;
                    s.volume = volume;
                    s.mute = mute;
                    s.Play();
                    Destroy(s.gameObject, 2f);
                    Destroy(particleIn.gameObject, 2f);
                    transform.parent.position += new Vector3(moveDist * factorX, moveDist * factorY, 0);
                    GameObject particleOut = Instantiate(portalOut.gameObject, transform.parent.position, transform.rotation);
                    particleOut.GetComponent<ParticleSystem>().Play();
                    Destroy(particleOut.gameObject, 2f);
                }
                moved = false;
            }
            else if (moved)
            {
                moved = false;
            }
        }
    }

    public override void Attack()
    {
        LocalTimersManager.CreateNewTimer(attackRecharge).DoAfter(() =>
        {
            wg.SpawnWave();
            AudioSource s = Instantiate(source, transform.parent.position, new Quaternion(0, 0, 0, 0)).GetComponent<AudioSource>();
            s.clip = attackClip;
            s.volume = volume;
            s.mute = mute;
            s.Play();
            Destroy(s.gameObject, 2f);
            canAttack = true;
        }).Start();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.parent.position, range);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.parent.position, escapeRange);
        if (player != null)
        {
            Gizmos.DrawLine(transform.parent.position, player.position);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.parent.position, new Vector3(transform.parent.position.x + moveDist * factorX, transform.parent.position.y + moveDist * factorY));
    }
}
