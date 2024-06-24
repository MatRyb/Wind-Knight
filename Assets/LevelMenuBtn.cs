using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenuBtn : MonoBehaviour
{
    public LevelData LevelData;

    public Image Star1;
    public Image Star2;
    public Image Star3;

    private void Update()
    {
        uint starsNum = LevelData.GetStars(PlayerPrefs.GetFloat(LevelData.Name + "Time", -1f));
        if (starsNum >= 1)
        {
            Star1.color = LevelData.ActiveStar;
        }
        else
        {
            Star1.color = LevelData.DeactiveStar;
        }

        if (starsNum >= 2)
        {
            Star2.color = LevelData.ActiveStar;
        }
        else
        {
            Star2.color = LevelData.DeactiveStar;
        }

        if (starsNum == 3)
        {
            Star3.color = LevelData.ActiveStar;
        }
        else
        {
            Star3.color = LevelData.DeactiveStar;
        }
    }
}
