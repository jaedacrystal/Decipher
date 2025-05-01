using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;
using Photon.Pun;

public class MultiCardManager : MonoBehaviourPunCallbacks
{
    [Header("Card Settings")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform hand;
    [SerializeField] private Transform deck;
    [SerializeField] public List<Cards> listOfCards;
    [SerializeField] private int maxHandSize;

    [Header("Card Counter")]
    public TextMeshProUGUI counterText;
    private int deckCounter;

    [Header("Players")]
    public GameObject myPlayer;
    public GameObject enemyPlayer;
    public GameObject player;

    private List<GameObject> cardInstances = new();
    private List<Cards> mergedDeck = new();
    private List<Cards> selectedClassCards = new();
    private List<Cards> opponentDeck = new();

    private bool isPlayerTurn = false;

    private void Start()
    {
        AssignPlayers();

        photonView.RPC("RPC_UpdatePlayerNames", RpcTarget.AllBuffered, PhotonNetwork.NickName);

        string savedClass = PlayerPrefs.GetString("ChosenClass", "None");
        if (savedClass != "None" && Enum.TryParse(savedClass, out ClassType chosenClass))
        {
            InitializeDeck(chosenClass);
            DrawMultipleCards(maxHandSize);
        }

        UpdateDeckCounter();
    }

    public void AssignPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            PhotonView view = p.GetComponent<PhotonView>();
            if (view != null)
            {
                if (view.IsMine)
                {
                    myPlayer = p;
                    Debug.Log($"Assigned myPlayer: {myPlayer.name}");

                    MultiplayerHealth health = myPlayer.GetComponent<MultiplayerHealth>();
                    if (health != null)
                    {
                        health.isPlayer = true;
                        health.isEnemyPlayer = false;
                        health.playerNameText.text = PhotonNetwork.NickName;
                        Debug.Log("Initialized MultiplayerHealth for local player.");
                    }
                }
                else
                {
                    enemyPlayer = p;
                    Debug.Log($"Assigned enemyPlayer: {enemyPlayer.name}");

                    MultiplayerHealth health = enemyPlayer.GetComponent<MultiplayerHealth>();
                    if (health != null)
                    {
                        health.isPlayer = false;
                        health.isEnemyPlayer = true;
                        health.enemyNameText.text = view.Owner.NickName;
                        Debug.Log("Initialized MultiplayerHealth for enemy player.");
                    }
                }
            }
        }

        if (enemyPlayer != null)
        {
            PhotonView enemyPhotonView = enemyPlayer.GetComponent<PhotonView>();
            if (enemyPhotonView != null && enemyPhotonView.IsMine)
            {
                Debug.LogWarning("Enemy PhotonView is incorrectly marked as mine. Fixing ownership.");
                enemyPhotonView.TransferOwnership(enemyPhotonView.Owner.ActorNumber);
            }
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            UpdateDeckCounter();
        }
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

        foreach (var card in mergedDeck)
        {
            card.isSingleplayer = GameManager.Instance.IsSingleplayer;
        }

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
            GameObject g = Instantiate(cardPrefab, deck);
            g.SetActive(false);

            CardDisplay cardDisplay = g.GetComponent<CardDisplay>();
            cardDisplay.SetCard(cardData);

            cardInstances.Add(g);
        }
    }

    [PunRPC]
    private void RPC_ReceiveOpponentDeck(string[] cardNames)
    {
        opponentDeck = new List<Cards>();

        foreach (string name in cardNames)
        {
            Cards found = listOfCards.Find(card => card.cardName == name);
            if (found != null)
            {
                opponentDeck.Add(found);
            }
            else
            {
                Debug.LogWarning($"Card {name} not found in listOfCards!");
            }
        }

        Debug.Log("Opponent deck received with " + opponentDeck.Count + " cards.");
    }


    private void SendDeckToOpponent()
    {
        string[] cardNames = new string[mergedDeck.Count];
        for (int i = 0; i < mergedDeck.Count; i++)
        {
            cardNames[i] = mergedDeck[i].cardName;
        }

        Photon.Pun.PhotonView photonView = Photon.Pun.PhotonView.Get(this);
        photonView.RPC("RPC_ReceiveOpponentDeck", Photon.Pun.RpcTarget.Others, cardNames);
    }


    public void DrawCard()
    {
        if (!photonView.IsMine) return;

        if (cardInstances.Count == 0) return;

        GameObject drawnCard = cardInstances[0];
        cardInstances.RemoveAt(0);

        drawnCard.transform.SetParent(hand);
        drawnCard.SetActive(true);

        drawnCard.transform.localScale = Vector3.zero;
        drawnCard.transform.DOScale(Vector3.one, 0.3f);

        Debug.Log($"[{PhotonNetwork.NickName}] drew card: {drawnCard.GetComponent<CardDisplay>().card.cardName}");

        photonView.RPC("RPC_DrawCardForEnemy", RpcTarget.Others);

        CardPosition();
        UpdateDeckCounter();
    }

    public void SetPlayerTurn(bool isTurn)
    {
        isPlayerTurn = isTurn;
    }

    public void DrawMultipleCards(int count)
    {
        if (!photonView.IsMine) return;
        if (!isPlayerTurn) return;

        for (int i = 0; i < count; i++)
        {
            if (cardInstances.Count == 0 || hand.childCount >= maxHandSize)
                break;

            DrawCard();
        }
    }



    [PunRPC]
    private void RPC_DrawCardForEnemy()
    {
        if (!photonView.IsMine && cardInstances.Count > 0)
        {
            GameObject drawnCard = cardInstances[0];
            cardInstances.RemoveAt(0);

            drawnCard.transform.SetParent(hand);
            drawnCard.SetActive(true);

            drawnCard.transform.localScale = Vector3.zero;
            drawnCard.transform.DOScale(Vector3.one, 0.3f);

            Debug.Log($"Enemy drew card: {drawnCard.GetComponent<CardDisplay>().card.cardName}");

            CardPosition();
            UpdateDeckCounter();
        }
    }

    public void HandleOpponentPlayedCard(string cardName)
    {
        Debug.Log($"Handling opponent's played card: {cardName}");

        Cards foundCard = opponentDeck.Find(card => card.cardName == cardName);

        if (foundCard != null)
        {
            Debug.Log($"Opponent played a real card: {foundCard.cardName}");
        }
        else
        {
            Debug.LogWarning($"Card {cardName} not found in opponent deck!");
        }
    }

    public void CardPosition()
    {
        int cardCount = hand.childCount;
        if (cardCount == 0) return;

        for (int i = 0; i < cardCount; i++)
        {
            float center = (cardCount - 1) / 2f;
            float interval = 100f;
            float x = (i - center) * interval;

            hand.GetChild(i).localPosition = new Vector3(x, 0, 0);
        }
    }

    public void RemoveCard(GameObject cardToRemove)
    {
        if (!photonView.IsMine) return;

        if (cardToRemove.transform.parent == hand)
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
        deckCounter = deck.childCount;
        counterText.text = deckCounter.ToString();

        if (deckCounter <= 0)
        {
            counterText.text = "0";
        }
    }

    [PunRPC]
    private void RPC_UpdatePlayerNames(string senderPlayerName)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var playerGO in players)
        {
            PhotonView playerView = playerGO.GetComponent<PhotonView>();

            if (playerView.Owner.NickName == senderPlayerName)
            {
                if (playerView.IsMine)
                {
                    MultiplayerHealth health = playerGO.GetComponent<MultiplayerHealth>();
                    if (health != null)
                    {
                        health.playerNameText.text = "You";
                    }
                }
                else
                {
                    MultiplayerHealth health = playerGO.GetComponent<MultiplayerHealth>();
                    if (health != null)
                    {
                        health.enemyNameText.text = senderPlayerName;
                    }
                }
            }
        }
    }

}
