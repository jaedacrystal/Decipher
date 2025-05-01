using UnityEngine;
using TMPro;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;

public class MultiTurnManager : MonoBehaviourPunCallbacks
{
    public static MultiTurnManager Instance;

    [Header("Player")]
    public bool isPlayerTurn = false;
    public PlayerStats playerStats;
    public MultiplayerHealth health;

    public GameObject myPlayer;
    public GameObject opponent;

    [Header("Game Objects")]
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI opponentCardText;
    public GameObject turnButton;

    private int currentTurnActorNumber;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        health = FindObjectOfType<MultiplayerHealth>();
        turnText.color = new Color(turnText.color.r, turnText.color.g, turnText.color.b, 1);
        DetermineStartingPlayer();
    }

    private void DetermineStartingPlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentTurnActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            photonView.RPC("BeginPlayerTurn", RpcTarget.All, currentTurnActorNumber);
        }
        else
        {
            turnButton.SetActive(false);
        }
    }

    [PunRPC]
    public void EndTurn()
    {
        if (!isPlayerTurn) return;

        isPlayerTurn = false;
        UpdateTurnUI();
        turnButton.SetActive(false);

        currentTurnActorNumber = GetNextPlayerActorNumber();
        photonView.RPC("BeginPlayerTurn", RpcTarget.All, currentTurnActorNumber);
    }

    [PunRPC]
    public void BeginPlayerTurn(int actorNumber)
    {
        if (health.currentHealth <= 0) return;

        // Update the current player's turn
        currentTurnActorNumber = actorNumber;
        isPlayerTurn = PhotonNetwork.LocalPlayer.ActorNumber == currentTurnActorNumber;

        UpdateTurnUI();  // Update the UI based on the turn

        // Handle turn logic
        if (isPlayerTurn)
        {
            photonView.RPC("RPC_RestoreBandwidth", RpcTarget.All, playerStats.maxBandwidth);

            MultiCardManager cardManager = FindObjectOfType<MultiCardManager>();
            if (cardManager != null)
            {
                cardManager.SetPlayerTurn(isPlayerTurn);
                cardManager.DrawMultipleCards(4);  // Draw cards when it's the player's turn
            }

            turnButton.SetActive(true);  // Show the turn button
        }
        else
        {
            MultiCardManager cardManager = FindObjectOfType<MultiCardManager>();
            if (cardManager != null)
            {
                cardManager.SetPlayerTurn(false);  // Set opponent's turn in the card manager
            }

            turnButton.SetActive(false);  // Hide the turn button if it's not the player's turn
        }
    }


    private int GetNextPlayerActorNumber()
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].ActorNumber == currentTurnActorNumber)
            {
                int nextIndex = (i + 1) % players.Length;
                return players[nextIndex].ActorNumber;
            }
        }

        return players[0].ActorNumber; // fallback
    }

    private void UpdateTurnUI()
    {
        if (health.currentHealth > 0)
        {
            turnText.text = isPlayerTurn ? "Your Turn" : "Opponent's Turn";
            turnText.color = new Color(turnText.color.r, turnText.color.g, turnText.color.b, 1);
            turnText.gameObject.SetActive(true);
            turnText.DOFade(1, 1f);

            Invoke("HideTurnText", 1);
        }
    }

    private void HideTurnText()
    {
        turnText.DOFade(0, 1f).OnComplete(() => turnText.gameObject.SetActive(false));
    }

    [PunRPC]
    public void DisplayPlayedCard(string playerName, string cardName)
    {
        if (health.currentHealth > 0 && opponentCardText != null)
        {
            if (playerName != PhotonNetwork.NickName)
            {
                opponentCardText.text = playerName + " played: " + cardName;
                AnimateText(opponentCardText);
                Invoke("HideOpponentCardText", 1.2f);
            }
        }
    }

    private void AnimateText(TextMeshProUGUI text)
    {
        text.gameObject.SetActive(true);
        text.transform.localScale = Vector3.zero;
        text.DOFade(1, 0.2f);
        text.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    private void HideOpponentCardText()
    {
        opponentCardText.DOFade(0, 1f).OnComplete(() => opponentCardText.gameObject.SetActive(false));
    }

    public void OnTurnButtonPressed()
    {
        if (isPlayerTurn)
        {
            photonView.RPC("EndTurn", RpcTarget.All);
        }
    }
}
