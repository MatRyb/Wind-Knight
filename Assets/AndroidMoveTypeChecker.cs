using CnControls;
using UnityEngine;

public class AndroidMoveTypeChecker : MonoBehaviour
{
    [SerializeField] private GameObject joystick;

#if UNITY_ANDROID
    void Start()
    {
        MoveType type = (MoveType)PlayerPrefs.GetInt("AndroidMoveType", (int)MoveType.Joystick);
        if (type == MoveType.Touch)
        {
            joystick.GetComponent<SimpleJoystick>().enabled = false;
            joystick.SetActive(false);
        }
        else if (type == MoveType.Joystick) 
        {
            joystick.GetComponent<SimpleJoystick>().enabled = true;
            joystick.SetActive(true);
        }
    }
#endif
}
