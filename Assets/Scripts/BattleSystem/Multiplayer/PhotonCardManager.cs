using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using Photon.Pun;

public class PhotonCardManager : MonoBehaviour
{
    public static PhotonCardManager Instance { get; private set; }

    [Header("Shared Settings")]
    [SerializeField] public GameObject cardPrefab;
    [SerializeField] public List<Cards> baseDeck;

    [Header("Player References")]
    [SerializeField] public GameObject playerHand;
    [SerializeField] public GameObject playerDeckObj;
    [SerializeField] public TextMeshProUGUI playerDeckCounter;

    [Header("Opponent References")]
    [SerializeField] public GameObject opponentHand;
    [SerializeField] public GameObject opponentDeckObj;
    [SerializeField] public TextMeshProUGUI opponentDeckCounter;

    [Header("Hand Settings")]
    [SerializeField] private int maxHandSize = 4;

    public GameObject localPlayer;
    public GameObject opponentPlayer;
    [HideInInspector] public GameObject player;
    [HideInInspector] public GameObject opponent;


    private List<Cards> playerDeckCards = new();
    private List<Cards> opponentDeckCards = new();
    private List<GameObject> playerCardInstances = new();
    private List<GameObject> opponentCardInstances = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        AssignPlayers();

        Photon.Realtime.Player localPlayerPhoton = PhotonNetwork.LocalPlayer;
        Photon.Realtime.Player opponentPhoton = GetOpponentPlayer();

        string playerClass = PlayerPrefs.GetString("ChosenClass", "None");
        if (Enum.TryParse(playerClass, out ClassType playerClassType))
        {
            playerDeckCards = BuildDeckForClass(playerClassType, "ChosenClassCards");
            InstantiateDeck(playerDeckCards, playerDeckObj, playerCardInstances);
            DrawMultipleCards(playerDeckObj, playerHand, playerCardInstances, maxHandSize);
        }

        if (opponentPhoton != null && opponentPhoton.CustomProperties.TryGetValue("playerClass", out object opponentClassObj))
        {
            if (Enum.TryParse(opponentClassObj.ToString(), out ClassType opponentClassType))
            {
                opponentDeckCards = BuildDeckForClass(opponentClassType, null);
                InstantiateDeck(opponentDeckCards, opponentDeckObj, opponentCardInstances);
                DrawMultipleCards(opponentDeckObj, opponentHand, opponentCardInstances, maxHandSize);
            }
        }

        UpdateDeckCounter(playerDeckObj, playerDeckCounter);
        UpdateDeckCounter(opponentDeckObj, opponentDeckCounter);
    }

    private void AssignPlayers()
    {
        PlayerStats[] allPlayers = FindObjectsOfType<PlayerStats>();

        foreach (var ps in allPlayers)
        {
            PhotonView view = ps.GetComponent<PhotonView>();
            if (view != null && view.IsMine)
            {
                localPlayer = ps.gameObject;
                player = localPlayer; // Set for CardDrag compatibility
            }
            else
            {
                opponentPlayer = ps.gameObject;
                opponent = opponentPlayer; // Set for CardDrag compatibility
            }
        }

        if (player == null)
            Debug.LogError("Local player not found in PhotonCardManager!");
        if (opponent == null)
            Debug.LogWarning("Opponent player not found in PhotonCardManager!");
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
            Debug.LogWarning($"No PlayerPrefsKey provided for {classType}. Using fallback.");
            return new List<Cards>(baseDeck);
        }

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
        text.text = $"{deckObj.transform.childCount}";
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

    public void DrawCardsForStartOfTurn()
    {
        DrawMultipleCards(playerDeckObj, playerHand, playerCardInstances, 4);
        UpdateDeckCounter(playerDeckObj, playerDeckCounter);
    }

    [System.Serializable]
    private class CardListWrapper
    {
        public List<Cards> cards;
    }
}
