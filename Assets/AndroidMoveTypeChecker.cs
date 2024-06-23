using CnControls;
using UnityEngine;

public class AndroidMoveTypeChecker : MonoBehaviour
{
    [SerializeField] private GameObject joystick;

#if UNITY_ANDROID
    void Start()
    {
        AndroidMoveType type = (AndroidMoveType)PlayerPrefs.GetInt("AndroidMoveType", (int)AndroidMoveType.Joystick);
        if (type == AndroidMoveType.Touch)
        {
            joystick.GetComponent<SimpleJoystick>().enabled = false;
            joystick.SetActive(false);
        }
        else if (type == AndroidMoveType.Joystick) 
        {
            joystick.GetComponent<SimpleJoystick>().enabled = true;
            joystick.SetActive(true);
        }
    }
#endif
}
