using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectManager : MonoBehaviour
{
    [SerializeField] private List<Transform> transformList = new List<Transform>();

    [SerializeField] private List<GameObject> prefabList = new List<GameObject>();

    [SerializeField] private int objectCountPerTransform = 20;

    void Awake()
    {
        foreach (Transform tran in transformList)
        {
            for(int i=0;i<objectCountPerTransform;i++)
            {
                StartCoroutine(Spawn(tran, i));
            }
        }
    }

    IEnumerator Spawn(Transform tran, int i)
    {
        yield return new WaitForSeconds(i);
        GameObject obj = Instantiate(prefabList[Random.Range(0, prefabList.Count)], tran.position, Quaternion.identity);
        obj.GetComponent<ObjectScript>().setFactor(0);
        obj.GetComponent<ObjectScript>().StartHealth();
        yield return new WaitForSeconds(2);
        obj.GetComponent<ObjectScript>().setFactor(1);
    }
}
