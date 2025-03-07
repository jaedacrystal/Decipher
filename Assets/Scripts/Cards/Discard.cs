using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class Discard : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject graveyard;

    public TextMeshProUGUI counterText;
    public int graveyardCounter;

    public void OnDrop(PointerEventData eventData)
    {
        CardDrag card = eventData.pointerDrag.GetComponent<CardDrag>();
        if (card != null)
        {
            CardManager cardManager = GetComponent<CardManager>();
            if (cardManager != null)
            {
                cardManager.RemoveCard(card.gameObject);
            }

            card.isDragging = true;
            card.transform.SetParent(graveyard.transform, false);
            card.Hide();

            graveyardCounter = graveyard.transform.childCount;
            counterText.text = graveyardCounter.ToString();

            for (int i = 0; i < graveyardCounter; i++)
            {
                counterText.transform.DOScale(0.5f, 0.2f);
            }

            counterText.transform.localScale = Vector3.zero;
        }

        Debug.Log("Card Dropped on " + gameObject.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CardDrag card = eventData.pointerDrag.GetComponent<CardDrag>();

        if (card != null)
        {
            card.transform.DOScale(0.5f, 0.2f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CardDrag card = eventData.pointerDrag.GetComponent<CardDrag>();

        if (card != null)
        {
            card.transform.DOScale(Vector3.one, 0.3f);
        }
    }
}
