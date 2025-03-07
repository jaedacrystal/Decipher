using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CanvasGroup canvasGroup;
    private Transform cardParent = null;
    public bool isDragging;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Card Dragged");

        isDragging = false;
        canvasGroup.blocksRaycasts = false;
        cardParent = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        transform.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)   
    {
        Discard discardCard = GetComponent<Discard>();
        canvasGroup.blocksRaycasts = true;

        if (isDragging)
        {
            discardCard.OnDrop(eventData);
        }
        else
        {
            ReturnToOriginalPosition();
        }

        Debug.Log("Card Drag Ended");

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ReturnToOriginalPosition()
    {
        Vector3 targetPosition = cardParent.localPosition;

        transform.DOLocalMove(targetPosition, 0.3f).OnComplete(() =>
        {
            this.transform.SetParent(cardParent);
        });
        
    }
}