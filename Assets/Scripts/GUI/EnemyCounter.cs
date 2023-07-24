using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class EnemyCounter : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    private List<EnemyController> enemies = new();
}
