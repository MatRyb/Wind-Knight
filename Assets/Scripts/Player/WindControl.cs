using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class WindControl : MonoBehaviour
{
    [SerializeField] private PlayerControler playerControler;
    [SerializeField] private float windBubbleRange = 4.5f;
    [SerializeField] private float windPower = 2f;
    [SerializeField] private float maxMovePower = 20f;

    [DisableIf("true")] [SerializeField] private List<ObjectInRange> objectsInRange;

    [System.Serializable]
    private class ObjectInRange
    {
        public GameObject gameObject;
        public Rigidbody2D rigidbody {
            get {
                Rigidbody2D rigid = null;
                if (gameObject != null)
                    gameObject.TryGetComponent(out rigid);
                return rigid;
            } 
        }
        public float gravityScale;
        public bool stillValid = true;

        public ObjectInRange(GameObject gameObject, float gravityScale)
        {
            this.gameObject = gameObject;
            this.gravityScale = gravityScale;
        }
    }

    private void OnValidate()
    {
        if (playerControler == null && GetComponent<PlayerControler>() != null)
        {
            playerControler = GetComponent<PlayerControler>();
        }
        else if (playerControler == null)
        {
            Debug.LogError("Player Controler can't be null. Please provide one. :)");
        }
    }

    private void Start()
    {
        if (playerControler == null)
            return;
    }

    void FixedUpdate()
    {
        if (playerControler == null)
            return;

        UpdateObjectsList();

        MoveObjects();
    }

    private void UpdateObjectsList()
    {
        foreach (var obj in objectsInRange)
        {
            obj.stillValid = false;
        }

        GetObjectsInRange();

        int removed = 0;
        for (int i = 0; i < objectsInRange.Count - removed; ++i)
        {
            if (!objectsInRange[i - removed].stillValid)
            {
                if (objectsInRange[i - removed].rigidbody != null)
                {
                    objectsInRange[i - removed].rigidbody.gravityScale = objectsInRange[i - removed].gravityScale;
                }
                objectsInRange.Remove(objectsInRange[i - removed]);
                ++removed;
            }
        }
    }

    private void GetObjectsInRange()
    {
        Collider2D[] collidersInRange = Physics2D.OverlapCircleAll(playerControler.playerBodyTransform.position, windBubbleRange);
        if (collidersInRange.Length != 0)
        {
            foreach (var collider in collidersInRange)
            {
                Rigidbody2D rigid;
                if (collider.TryGetComponent<Rigidbody2D>(out rigid))
                {
                    if (rigid != playerControler.playerRigidbody && objectsInRange.Find((o) => o.gameObject == rigid.gameObject) == null)
                    {
                        objectsInRange.Add(new ObjectInRange(rigid.gameObject, rigid.gravityScale));
                    }
                    else if (objectsInRange.Find((o) => o.gameObject == rigid.gameObject) != null)
                    {
                        ObjectInRange obj = objectsInRange.Find((o) => o.gameObject == rigid.gameObject);
                        obj.stillValid = true;
                    }
                }
            }
        }
    }

    private void MoveObjects()
    {
        foreach (var obj in objectsInRange)
        {
            if (obj.rigidbody != null)
            {
                if (playerControler.playerState == PlayerState.MOVING)
                {
                    obj.rigidbody.gravityScale = 0f;

                    float percent = (Mathf.Abs(Vector2.Distance(obj.gameObject.transform.position, playerControler.playerBodyTransform.position)) - (windBubbleRange / 2f)) / (windBubbleRange / 2f);
                    Vector2 additionForce = (playerControler.playerBodyTransform.position - obj.gameObject.transform.position).normalized * percent * Time.fixedDeltaTime * windPower;

                    Vector2 newVelocityDirection = ((playerControler.velocity - obj.rigidbody.velocity) / obj.rigidbody.mass) * Time.fixedDeltaTime * windPower;

                    if (Vector2.Distance(Vector2.zero, newVelocityDirection) >= maxMovePower)
                    {
                        newVelocityDirection = newVelocityDirection.normalized * maxMovePower;
                    }

                    obj.rigidbody.velocity += (newVelocityDirection + additionForce);
                }
                else
                {
                    obj.rigidbody.gravityScale = obj.gravityScale;
                }

                obj.rigidbody.velocity = obj.rigidbody.velocity * GameTimer.timeMultiplayer;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 playerPos = playerControler.playerBodyTransform.position;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerPos, windBubbleRange);
    }
}
