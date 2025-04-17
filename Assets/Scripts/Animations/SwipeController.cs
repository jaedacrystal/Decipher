using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwipeController : MonoBehaviour
{
    [SerializeField] private int maxPage = 3;
    [SerializeField] private int currentPage = 0;
    private Vector3 targetPos;

    [SerializeField] public Vector3 pageStep;
    [SerializeField] public RectTransform pagesRect;
    [SerializeField] float duration;

    public TextMeshProUGUI className;

    private void Awake()
    {
        currentPage = 1;
        targetPos = pagesRect.localPosition;
    }

    private void Update()
    {
        if (currentPage == 1)
        {
            className.text = "Offense";
        } else if (currentPage == 2)
        {
            className.text = "Balanced";
        } else if (currentPage == 3)
        {
            className.text = "Defense";
        }
    }

    public void Next()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
            targetPos += pageStep;
            MovePage();
        }
    }

    public void Previous()
    {
        if (currentPage > 1)
        {
            currentPage--;
            targetPos -= pageStep;
            MovePage();
        }
    }

    void MovePage()
    {
        pagesRect.LeanMoveLocal(targetPos, duration).setEaseOutCirc();
    }
}
