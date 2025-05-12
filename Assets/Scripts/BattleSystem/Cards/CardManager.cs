using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

public class CardManager : MonoBehaviour
{
    [Header("Card Settings")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] public GameObject hand;
    [SerializeField] private GameObject deck;
    [SerializeField] public List<Cards> listOfCards;
    [SerializeField] private int maxHandSize;
    public GameObject player;
    public GameObject opponent;

    [Header("Card Counter")]
    public TextMeshProUGUI counterText;
    public int deckCounter;

    private List<GameObject> cardInstances = new();
    private List<Cards> mergedDeck = new();
    private List<Cards> selectedClassCards = new();
    public List<Cards> opponentDeck;
    private Cards reusableCard;

    private void Start()
    {
        string savedClass = PlayerPrefs.GetString("ChosenClass", "None");
        if (savedClass == "None") return;

        if (Enum.TryParse(savedClass, out ClassType chosenClass))
        {
            InitializeDeck(chosenClass);
        }
        UpdateDeckCounter();
        DrawMultipleCards(maxHandSize);
    }

    private void Update()
    {
        UpdateDeckCounter();
    }

    public void InitializeDeck(ClassType chosenClass)
    {
        List<Cards> classCards = LoadClassCardsFromPrefs();

        if (classCards == null || classCards.Count < 3) return;

        selectedClassCards = new List<Cards>();
        HashSet<int> selectedIndexes = new HashSet<int>();

        while (selectedClassCards.Count < 3)
        {
            int randomIndex = UnityEngine.Random.Range(0, classCards.Count);
            if (selectedIndexes.Add(randomIndex))
            {
                selectedClassCards.Add(classCards[randomIndex]);
            }
        }

        mergedDeck = new List<Cards>(selectedClassCards);
        mergedDeck.AddRange(listOfCards);

        ShuffleDeck();
        InstantiateDeckCards();
        UpdateDeckCounter();
    }

    private void ShuffleDeck()
    {
        for (int i = mergedDeck.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (mergedDeck[i], mergedDeck[randomIndex]) = (mergedDeck[randomIndex], mergedDeck[i]);
        }
    }

    private void InstantiateDeckCards()
    {
        foreach (var obj in cardInstances)
        {
            Destroy(obj);
        }
        cardInstances.Clear();

        foreach (var cardData in mergedDeck)
        {
            GameObject g = Instantiate(cardPrefab, deck.transform);
            g.SetActive(false);

            CardDisplay cardDisplay = g.GetComponent<CardDisplay>();
            cardDisplay.SetCard(cardData);

            cardInstances.Add(g);
        }

        foreach (Cards card in listOfCards)
        {
            Cards instantiatedCard = Instantiate(card);
            instantiatedCard.isSingleplayer = GameManager.Instance.IsSingleplayer;
            selectedClassCards.Add(instantiatedCard);
        }
    }

    public void DrawCard()
    {
        GameObject drawnCard = cardInstances[0];
        cardInstances.RemoveAt(0);

        drawnCard.transform.SetParent(hand.transform);
        drawnCard.SetActive(true);

        drawnCard.transform.localScale = Vector3.zero;
        drawnCard.transform.DOScale(Vector3.one, 0.3f);

        Debug.Log($"Player drew card: {drawnCard.GetComponent<CardDisplay>().card.cardName}");
        CardPosition();
        UpdateDeckCounter();
    }

    public void DrawMultipleCards(int count)
    {
        Debug.Log($"Drawing {count} cards...");
        for (int i = 0; i < count; i++)
        {
            if (cardInstances.Count == 0 || hand.transform.childCount >= maxHandSize)
            {
                Debug.LogWarning("Cannot draw more cards. Either deck is empty or hand is full.");
                break;
            }
            DrawCard();
        }
    }

    public void CardPosition()
    {
        int cardCount = hand.transform.childCount;
        if (cardCount == 0) return;

        for (int i = 0; i < cardCount; i++)
        {
            float center = (cardCount - 1) / 2f;
            float interval = 100f;
            float x = (i - center) * interval;

            hand.transform.GetChild(i).localPosition = new Vector3(x, 0, 0);
        }
    }

    public void RemoveCard(GameObject cardToRemove)
    {
        if (cardToRemove.transform.parent == hand.transform)
        {
            Destroy(cardToRemove);
            CardPosition();
        }
    }

    private List<Cards> LoadClassCardsFromPrefs()
    {
        string cardsJson = PlayerPrefs.GetString("ChosenClassCards", "");
        if (!string.IsNullOrEmpty(cardsJson))
        {
            CardListWrapper wrapper = JsonUtility.FromJson<CardListWrapper>(cardsJson);
            return wrapper?.cards ?? new List<Cards>();
        }
        return new List<Cards>();
    }

    [System.Serializable]
    private class CardListWrapper
    {
        public List<Cards> cards;
    }
    private void UpdateDeckCounter()
    {
        deckCounter = deck.transform.childCount;
        counterText.text = deckCounter.ToString();

        if (deckCounter <= 0)
        {
            counterText.text = "0";
        }
    }

    public void DiscardCardFromOpponentDeck()
    {
        if (opponentDeck.Count == 0) return;

        int randomIndex = UnityEngine.Random.Range(0, opponentDeck.Count);
        Cards cardToDiscard = opponentDeck[randomIndex];

        opponentDeck.RemoveAt(randomIndex);

        Debug.Log("Opponent discarded a card: " + cardToDiscard.cardName);
    }
}

