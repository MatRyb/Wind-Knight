using UnityEngine;

public class PhoneCanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject phoneCanvasRight;
    [SerializeField] private GameObject phoneCanvasLeft;
    [SerializeField] private GameObject computerPowerUp;
    private void Start()
    {
#if UNITY_ANDROID

        AndroidArrangement arrangement = (AndroidArrangement)PlayerPrefs.GetInt("AndroidArrangement", (int)AndroidArrangement.RightHanded);

        if (arrangement == AndroidArrangement.RightHanded) 
        {
            phoneCanvasRight.SetActive(true);
            phoneCanvasLeft.SetActive(false);
        }
        else 
        {
            phoneCanvasRight.SetActive(false);
            phoneCanvasLeft.SetActive(true);
        }
        computerPowerUp.SetActive(false);
#elif UNITY_STANDALONE
        phoneCanvas.SetActive(false);
        computerPowerUp.SetActive(false);
#endif
    }
}
