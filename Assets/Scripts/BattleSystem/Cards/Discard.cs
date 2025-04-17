using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class Discard : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject graveyard;

    public TextMeshProUGUI counterText;
    public int graveyardCounter;

    public CardDrag card;

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
            card.gameObject.SetActive(false);

            UpdateGraveyardCounter();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        CardDrag card = eventData.pointerDrag.GetComponent<CardDrag>();

        card.transform.DOScale(0.5f, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        CardDrag card = eventData.pointerDrag.GetComponent<CardDrag>();

        card.transform.DOScale(Vector3.one, 0.3f);

    }

    public void UpdateGraveyardCounter()
    {
        graveyardCounter = graveyard.transform.childCount;
        counterText.text = graveyardCounter.ToString();

        for (int i = 0; i < graveyardCounter; i++)
        {
            counterText.transform.DOScale(0.6f, 0.2f);
        }

        counterText.transform.localScale = Vector3.zero;
    }
}

