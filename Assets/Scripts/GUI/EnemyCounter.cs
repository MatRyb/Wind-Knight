using UnityEngine;
using TMPro;

public class EnemyCounter : MonoBehaviour
{
    public TextMeshProUGUI textUI;

    private void Update()
    {
        EnemyController[] enemys = GameObject.FindObjectsOfType<EnemyController>();
        textUI.text = "Enemys Left: " + enemys.Length;
    }
}
