using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<Cards> listOfCards;
    [SerializeField] private GameObject hand;
    [SerializeField] private GameObject deck;
    [SerializeField] private int maxHandSize;

    public List<GameObject> card = new();

    private void Start()
    {
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        for (int i = 0; i < maxHandSize; i++)
        {
            int randomIndex = Random.Range(0, listOfCards.Count);
            Cards selectedCardData = listOfCards[randomIndex];

            GameObject g = Instantiate(cardPrefab, deck.transform);
            g.SetActive(false);

            CardDisplay cardDisplay = g.GetComponent<CardDisplay>();
            if (cardDisplay != null)
            {
                cardDisplay.SetCard(selectedCardData);
            }

            card.Add(g);
        }
    }

    public void DrawCard()
    {
        if (card.Count == 0) return;

        GameObject g = card[0];
        card.RemoveAt(0);

        g.transform.SetParent(hand.transform);
        g.SetActive(true);
        g.transform.localScale = Vector3.zero;
        g.transform.DOScale(Vector3.one, 0.3f);

        CardPosition();
    }

    public GameObject Spawn()
    {
        return Instantiate(cardPrefab, transform);
    }

    public void CardPosition()
    {
        if (card.Count == 0) return;

        for (int i = 0; i < card.Count; i++)
        {
            float center = (card.Count - 1) / 2f;
            float interval = 100f;
            float x = (i - center) * interval;

            card[i].transform.localPosition = new Vector3(x, 0, 0);
        }
    }

    public void RemoveCard(GameObject cardToRemove)
    {
        if (card.Contains(cardToRemove))
        {
            card.Remove(cardToRemove);
            CardPosition();
        }
    }
}










