using UnityEngine;

[System.Serializable]
public class PlayerColorMapping : ColorMapping
{    
    public PlayerColorMapping() : base()
    {
        color = Color.white;
        type = MappingType.Pattern;
        name = "Player";
        player = true;
    }
}
