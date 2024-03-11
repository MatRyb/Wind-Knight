using UnityEngine;

public class PhoneCanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject phoneCanvas;
    [SerializeField] private GameObject computerPowerUp;
    private void Start()
    {
#if UNITY_ANDROID
        phoneCanvas.SetActive(true);
        computerPowerUp.SetActive(false);
#elif UNITY_STANDALONE
        phoneCanvas.SetActive(false);
        computerPowerUp.SetActive(false);
#endif
    }
}
