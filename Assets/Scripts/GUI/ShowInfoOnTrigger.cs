using UnityEngine;

public class ShowInfoOnTrigger : MonoBehaviour
{
    [SerializeField] private string info;
    private GameObject infoObj = null;
    private bool isShowing = false;

    private void Update()
    {
        if (isShowing && Input.GetKeyDown(KeyCode.Escape))
        {
            isShowing = false;
            Destroy(infoObj);
            LevelManager.ResumeGame();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            infoObj = GUIManager.ShowInfo(info);
            infoObj.GetComponent<InfoGUIObject>().SetInfoText(info);
            LevelManager.PauseGame();
            isShowing = true;
        }
    }
}
