using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyWinCondition : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    [SerializeField] private GameObject blockingWall = null;
    private List<EnemyController> enemies = new();
    private bool inizialized = false;

    void Update()
    {
        int deleted = 0;
        for (int i = 0; i < enemies.Count - deleted; i++)
        {
            if (enemies[i] == null)
            {
                enemies.RemoveAt(i - deleted);
                deleted++;
            }
        }

        textUI.text = "Enemies Left: " + enemies.Count;

        if (enemies.Count == 0 && inizialized)
        {
            Destroy(blockingWall);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemy;
        if (collision.TryGetComponent<EnemyController>(out enemy))
        {
            if (!enemies.Contains(enemy))
            {
                enemies.Add(enemy);
                inizialized = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyController>(out EnemyController enemy))
        {
            if (!enemies.Contains(enemy))
            {
                enemies.Add(enemy);
                inizialized = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyController>(out EnemyController enemy))
        {
            if (enemies.Contains(enemy))
            {
                enemies.Remove(enemy);
            }
        }
    }
}
