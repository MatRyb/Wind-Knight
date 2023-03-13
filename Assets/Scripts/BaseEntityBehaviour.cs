using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntityBehaviour : MonoBehaviour
{
    /// <summary>
    /// Function which tells if detected object is behind some object
    /// </summary>
    /// <param name="objectToCheck">object which was detected</param>
    /// <returns>bool value, if true object is behind other object else object is not behind anything</returns>
    public bool isObjectBlockedByOtherObject(GameObject objectToCheck)
    {
        return isObjectBlockedByOtherObject(objectToCheck, null, ~0);
    }

    /// <summary>
    /// Function which tells if detected object is behind some object
    /// </summary>
    /// <param name="objectToCheck">object which was detected</param>
    /// <param name="blockingLayers">layers of objects behind which objectToCheck can hide</param>
    /// <returns>bool value, if true object is behind other object else object is not behind anything</returns>
    public bool isObjectBlockedByOtherObject(GameObject objectToCheck, LayerMask blockingLayers)
    {
        return isObjectBlockedByOtherObject(objectToCheck, null, blockingLayers);
    }

    /// <summary>
    /// Function which tells if detected object is behind some object
    /// </summary>
    /// <param name="objectToCheck">object which was detected</param>
    /// <param name="excludedTags">tags of objects which will be ignored while checking if objectToCheck is behind them</param>
    /// <returns>bool value, if true object is behind other object else object is not behind anything</returns>
    public bool isObjectBlockedByOtherObject(GameObject objectToCheck, string[] excludedTags)
    {
        return isObjectBlockedByOtherObject(objectToCheck, excludedTags, ~0);
    }

    /// <summary>
    /// Function which tells if detected object is behind some object
    /// </summary>
    /// <param name="objectToCheck">object which was detected</param>
    /// <param name="excludedTags">tags of objects which will be ignored while checking if objectToCheck is behind them</param>
    /// <param name="blockingLayers">layers of objects behind which objectToCheck can hide</param>
    /// <returns>bool value, if true object is behind other object else object is not behind anything</returns>
    public bool isObjectBlockedByOtherObject(GameObject objectToCheck, string[] excludedTags, LayerMask blockingLayers)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, (objectToCheck.transform.position - transform.position).normalized, Mathf.Abs(Vector3.Distance(objectToCheck.transform.position, transform.position)));
        if (hits.Length == 0)
        {
            return true;
        }

        List<RaycastHit2D> allowedHits = new List<RaycastHit2D>();
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject == this.gameObject)
                continue;

            bool next = false;
            if (excludedTags != null)
            {
                foreach (var tag in excludedTags)
                {
                    if (hit.collider.tag == tag)
                    {
                        next = true;
                        break;
                    }
                }

                if (next)
                    continue;
            }

            int layerVal = 1 << hit.collider.gameObject.layer;
            if ((blockingLayers | layerVal) != blockingLayers)
            {
                continue;
            }

            allowedHits.Add(hit);
        }
        if (allowedHits.Count == 0)
        {
            return true;
        }

        RaycastHit2D closestHit = allowedHits[0];
        float minDist = Mathf.Abs(Vector3.Distance(closestHit.point, transform.position));
        for (int i = 1; i < allowedHits.Count; i++)
        {
            float dist = Mathf.Abs(Vector3.Distance(allowedHits[i].point, transform.position));
            if (dist < minDist)
            {
                minDist = dist;
                closestHit = allowedHits[i];
            }
        }
        return closestHit.collider.gameObject != objectToCheck;
    }
}
