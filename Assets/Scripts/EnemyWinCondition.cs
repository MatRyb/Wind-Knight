using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyWinCondition : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    [SerializeField] private GameObject blockingWall = null;
    private List<EnemyController> enemys = new List<EnemyController>();
    private bool inizialized = false;

    void Update()
    {
        int deleted = 0;
        for (int i = 0; i < enemys.Count - deleted; i++)
        {
            if (enemys[i] == null)
            {
                enemys.RemoveAt(i - deleted);
                deleted++;
            }
        }

        textUI.text = "Enemys Left: " + enemys.Count;

        if (enemys.Count == 0 && inizialized)
        {
            Destroy(blockingWall);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemy;
        if (collision.TryGetComponent<EnemyController>(out enemy))
        {
            if (!enemys.Contains(enemy))
            {
                enemys.Add(enemy);
                inizialized = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        EnemyController enemy;
        if (collision.TryGetComponent<EnemyController>(out enemy))
        {
            if (!enemys.Contains(enemy))
            {
                enemys.Add(enemy);
                inizialized = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        EnemyController enemy;
        if (collision.TryGetComponent<EnemyController>(out enemy))
        {
            if (enemys.Contains(enemy))
            {
                enemys.Remove(enemy);
            }
        }
    }
}
