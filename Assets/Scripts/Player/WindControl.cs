using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class WindControl : MonoBehaviour
{
    [SerializeField] private PlayerControler playerControler;
    [SerializeField] private float windBubbleRange = 4.5f;

    [DisableIf("true")] [SerializeField] private List<ObjectInRange> objectsInRange;

    //private Vector2 lastMoveDirectionNormal = Vector2.right;

    [System.Serializable]
    private class ObjectInRange
    {
        public GameObject gameObject;
        public Rigidbody2D rigidbody {
            get {
                return gameObject.GetComponent<Rigidbody2D>();
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

        //lastMoveDirectionNormal = playerControler.velocity.normalized;
    }

    void Update()
    {
        if (playerControler == null)
            return;

        /*float angle = AdvancedMath.GetAngleBetweenPoints(Vector2.zero, -lastMoveDirectionNormal, playerControler.velocity.normalized);
        if (angle <= 30f && angle >= -30f)
        {

        }*/

        UpdateObjectsList();

        MoveObjects();

        //lastMoveDirectionNormal = playerControler.velocity.normalized;
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
                objectsInRange[i - removed].rigidbody.gravityScale = objectsInRange[i - removed].gravityScale;
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
            if (playerControler.playerState == PlayerState.MOVING)
            {
                obj.rigidbody.gravityScale = 0f;

                float percent = (Mathf.Abs(Vector2.Distance(obj.gameObject.transform.position, playerControler.playerBodyTransform.position)) - (windBubbleRange / 2f)) / (windBubbleRange / 2f);
                Vector2 additionForce = (playerControler.playerBodyTransform.position - obj.gameObject.transform.position).normalized * percent;

                obj.rigidbody.velocity = playerControler.velocity + additionForce;
            }
            else
            {
                obj.rigidbody.gravityScale = obj.gravityScale;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 playerPos = playerControler.playerBodyTransform.position;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerPos, windBubbleRange);

        /*Gizmos.color = Color.red;
        Gizmos.DrawLine(playerPos, playerPos + playerControler.velocity.normalized * 5);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(playerPos, playerPos - playerControler.velocity.normalized * 5);

        Gizmos.color = Color.blue;
        float r = Mathf.Abs(Vector2.Distance(playerPos - playerControler.velocity.normalized * 5, playerPos));
        float angle = AdvancedMath.GetAngleBetweenPoints(Vector2.zero, -playerControler.velocity.normalized, Vector2.right);
        float radian = (angle + 30f) * Mathf.PI / 180f;
        float x = Mathf.Cos(radian) * r;
        float y = Mathf.Sin(radian) * r;
        Gizmos.DrawLine(playerPos, playerPos + new Vector2(x, y));
        radian = (angle - 30f) * Mathf.PI / 180f;
        x = Mathf.Cos(radian) * r;
        y = Mathf.Sin(radian) * r;
        Gizmos.DrawLine(playerPos, playerPos + new Vector2(x, y));*/
    }
}
