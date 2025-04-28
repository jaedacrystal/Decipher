using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeController : MonoBehaviour, IEndDragHandler
{
    [SerializeField] private int maxPage = 3;
    [SerializeField] private int currentPage = 0;
    private Vector3 targetPos;

    [SerializeField] public Vector3 pageStep;
    [SerializeField] public RectTransform pagesRect;
    [SerializeField] float duration;

    private float dragThreshold;

    public TextMeshProUGUI className;
    public Class selectPrompt;

    private void Start()
    {
        selectPrompt = FindObjectOfType<Class>();
    }

    private void Awake()
    {
        currentPage = 1;
        targetPos = pagesRect.localPosition;
        dragThreshold = Screen.width / 15;
    }

    private void Update()
    {
        if (className == null) return;

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
        selectPrompt.NoButton();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.position.x - eventData.pressPosition.x) > dragThreshold)
        {
            if(eventData.position.x > eventData.pressPosition.x) Previous();
            else Next();
        } else
        {
            MovePage();
        }
    }
}
