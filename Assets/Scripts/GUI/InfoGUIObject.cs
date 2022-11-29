using UnityEngine;
using TMPro;

public class InfoGUIObject : MonoBehaviour
{
    [SerializeField] private RectTransform infoBox;
    [SerializeField] private TextMeshProUGUI infoText;

    public void SetInfoText(string text)
    {
        infoText.color = Color.black;
        infoText.text = text;
        infoText.ForceMeshUpdate();
        Vector2 textSize = infoText.GetRenderedValues(false);
        float paddingY = 10f;
        float paddingX = 70f;
        infoBox.sizeDelta = new Vector2(textSize.x + paddingX > 0 ? textSize.x + paddingX : paddingX, textSize.y + paddingY > 0 ? textSize.y + paddingY : paddingY);
    }
}
