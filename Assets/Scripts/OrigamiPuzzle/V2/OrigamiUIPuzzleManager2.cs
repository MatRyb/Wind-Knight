using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrigamiUIPuzzleManager2 : MonoBehaviour
{
    [SerializeField] private Camera UICamera;
    [SerializeField] private GameObject UIObject;

    public int currentOrderIndex = 0;
    public List<FoldPaper> foldPapers;

    public AnimationCurve rotCurve;

    public bool clickeBlocked = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !clickeBlocked)
        {
            if (Physics.Raycast(UICamera.ScreenToWorldPoint(Input.mousePosition), UICamera.transform.forward, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent<ClickDetector>(out ClickDetector detector))
                {
                    FoldPaper foldPaper = detector.foldPaperRef;
                    if (!foldPaper.rotated)
                    {
                        foldPaper.RotatePaper(false);
                    }
                    else
                    {
                        if (foldPaper.orderIndex < currentOrderIndex - 1)
                        {
                            StartCoroutine(RotateBackToIndex(foldPaper.orderIndex));
                        }
                        else
                        {
                            foldPaper.RotatePaper(true);
                        }
                    }
                }
            }
        }
    }

    IEnumerator RotateBackToIndex(int index)
    {
        int tmpIndex = currentOrderIndex;
        while (currentOrderIndex != index) 
        {
            if (tmpIndex == currentOrderIndex)
            {
                foldPapers[tmpIndex - 1].RotatePaper(true);
                tmpIndex--;
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public void SetUIOn()
    {
        LevelManager.PauseGame(false);
        UIObject.SetActive(true);
    }

    public void SetUIOff()
    {
        LevelManager.ResumeGame(false);
        UIObject.SetActive(false);
    }

    public bool IsUIOn()
    {
        return UIObject.activeSelf;
    }
}
