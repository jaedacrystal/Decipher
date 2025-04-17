using UnityEngine;
using UnityEngine.EventSystems;

public class PlayArea : MonoBehaviour, IDropHandler
{
    [SerializeField] private GameObject graveyard;
    private CardManager cardManager;
    private Discard discard;

    private void Start()
    {
        cardManager = FindObjectOfType<CardManager>();
        discard = FindObjectOfType<Discard>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        CardDrag cardDrag = eventData.pointerDrag?.GetComponent<CardDrag>();
        if (cardDrag == null) return;

        CardDisplay cardDisplay = cardDrag.GetComponent<CardDisplay>();
        if (cardDisplay == null || cardDisplay.card == null) return;

        Debug.Log($"Card {cardDisplay.card.cardName} was played!");

        GameObject player = cardManager.player;
        GameObject opponent = cardManager.opponent;
        cardDisplay.card.ApplyEffect(player, opponent);

        MoveToGraveyard(cardDrag);
    }

    private void MoveToGraveyard(CardDrag card)
    {
        card.transform.SetParent(graveyard.transform, false);
        card.gameObject.SetActive(false);

        discard.UpdateGraveyardCounter();
    }
}
