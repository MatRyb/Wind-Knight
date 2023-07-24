using UnityEngine;

public class TestDie : MonoBehaviour
{
    [SerializeField] private float dieTime = 1f;

    private void Start()
    {
        Destroy(gameObject, dieTime);
    }
}
