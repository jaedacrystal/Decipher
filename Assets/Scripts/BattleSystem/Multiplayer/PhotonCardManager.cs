using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using Photon.Pun;

public class PhotonCardManager : MonoBehaviour
{
    [Header("Shared Settings")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private List<Cards> baseDeck;

    [Header("Player References")]
    [SerializeField] private GameObject playerHand;
    [SerializeField] private GameObject playerDeckObj;
    [SerializeField] private TextMeshProUGUI playerDeckCounter;

    [Header("Opponent References")]
    [SerializeField] private GameObject opponentHand;
    [SerializeField] private GameObject opponentDeckObj;
    [SerializeField] private TextMeshProUGUI opponentDeckCounter;

    [Header("Hand Settings")]
    [SerializeField] private int maxHandSize = 4;

    private List<Cards> playerDeckCards = new();
    private List<Cards> opponentDeckCards = new();
    private List<GameObject> playerCardInstances = new();
    private List<GameObject> opponentCardInstances = new();

    private void Start()
    {
        Photon.Realtime.Player localPlayer = PhotonNetwork.LocalPlayer;
        Photon.Realtime.Player opponentPlayer = GetOpponentPlayer();

        string playerClass = PlayerPrefs.GetString("ChosenClass", "None");
        if (Enum.TryParse(playerClass, out ClassType playerClassType))
        {
            playerDeckCards = BuildDeckForClass(playerClassType, "ChosenClassCards");
            InstantiateDeck(playerDeckCards, playerDeckObj, playerCardInstances);
            DrawMultipleCards(playerDeckObj, playerHand, playerCardInstances, maxHandSize);
        }

        // Opponent's class (from Photon custom properties)
        if (opponentPlayer != null && opponentPlayer.CustomProperties.TryGetValue("playerClass", out object opponentClassObj))
        {
            if (Enum.TryParse(opponentClassObj.ToString(), out ClassType opponentClassType))
            {
                opponentDeckCards = BuildDeckForClass(opponentClassType, null); // No PlayerPrefs fallback
                InstantiateDeck(opponentDeckCards, opponentDeckObj, opponentCardInstances);
                DrawMultipleCards(opponentDeckObj, opponentHand, opponentCardInstances, maxHandSize);
            }
        }

        UpdateDeckCounter(playerDeckObj, playerDeckCounter);
        UpdateDeckCounter(opponentDeckObj, opponentDeckCounter);
    }


    private List<Cards> BuildDeckForClass(ClassType classType, string playerPrefsKey)
    {
        List<Cards> selectedClassCards = new();

        if (!string.IsNullOrEmpty(playerPrefsKey))
        {
            selectedClassCards = LoadClassCardsFromPrefs(playerPrefsKey);
        }
        else
        {
            // You can create a fallback here or load from Resources by classType
            Debug.LogWarning($"No PlayerPrefsKey provided for {classType}. Using fallback.");
            return new List<Cards>(baseDeck); // Or some other default
        }

        // Pick 3 random class cards
        HashSet<int> selectedIndexes = new();
        List<Cards> classCardsSubset = new();
        while (classCardsSubset.Count < 3 && selectedIndexes.Count < selectedClassCards.Count)
        {
            int index = UnityEngine.Random.Range(0, selectedClassCards.Count);
            if (selectedIndexes.Add(index))
            {
                classCardsSubset.Add(selectedClassCards[index]);
            }
        }

        List<Cards> fullDeck = new(classCardsSubset);
        fullDeck.AddRange(baseDeck);
        ShuffleDeck(fullDeck);
        return fullDeck;
    }

    private Photon.Realtime.Player GetOpponentPlayer()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.IsLocal) return player;
        }
        return null;
    }

    private void InstantiateDeck(List<Cards> cardList, GameObject deckObj, List<GameObject> instanceList)
    {
        foreach (var cardData in cardList)
        {
            GameObject g = Instantiate(cardPrefab, deckObj.transform);
            g.SetActive(false);
            g.GetComponent<CardDisplay>().SetCard(cardData);
            instanceList.Add(g);
        }
    }

    private void DrawMultipleCards(GameObject deckObj, GameObject handObj, List<GameObject> cardList, int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (deckObj.transform.childCount == 0 || handObj.transform.childCount >= maxHandSize) break;

            GameObject card = cardList[0];
            cardList.RemoveAt(0);

            card.transform.SetParent(handObj.transform);
            card.SetActive(true);
            card.transform.localScale = Vector3.zero;
            card.transform.DOScale(Vector3.one, 0.3f);
        }
    }

    private void ShuffleDeck(List<Cards> deck)
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int rand = UnityEngine.Random.Range(0, i + 1);
            (deck[i], deck[rand]) = (deck[rand], deck[i]);
        }
    }

    private void UpdateDeckCounter(GameObject deckObj, TextMeshProUGUI text)
    {
        text.text = $"Deck: {deckObj.transform.childCount}";
    }

    private List<Cards> LoadClassCardsFromPrefs(string prefsKey)
    {
        string cardsJson = PlayerPrefs.GetString(prefsKey, "");
        if (!string.IsNullOrEmpty(cardsJson))
        {
            return JsonUtility.FromJson<CardListWrapper>(cardsJson)?.cards ?? new List<Cards>();
        }
        return new List<Cards>();
    }


    [System.Serializable]
    private class CardListWrapper
    {
        public List<Cards> cards;
    }
}
