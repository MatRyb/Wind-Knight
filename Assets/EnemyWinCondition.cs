using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWinCondition : MonoBehaviour
{
    [SerializeField] private GameObject blockingWall = null;
    private List<EnemyController> enemys = new List<EnemyController>();

    void Update()
    {
        enemys.Clear();
        enemys.AddRange(GameObject.FindObjectsOfType<EnemyController>());

        if (enemys.Count == 0)
        {
            Destroy(blockingWall);
        }
    }
}
