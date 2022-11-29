using UnityEngine;
using UnityEngine.UI;

public class HealthMonitor : MonoBehaviour
{
    private ObjectHealth player;
    [SerializeField] private Image image;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<ObjectHealth>();
    }

    void Update()
    {
        image.fillAmount = player.getHealth() / player.getMaxHealth();
    }
}
