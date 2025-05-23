using UnityEngine;
using TMPro;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;

public class PhotonTurnManager : MonoBehaviourPunCallbacks
{
    public static PhotonTurnManager Instance;

    [Header("Player")]
    public bool isPlayerTurn = false;
    public PlayerStats playerStats;
    public Health health;

    [Header("Game Objects")]
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI opponentCardText;
    public GameObject turnButton;

    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI opponentNameText;

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
        health = FindObjectOfType<Health>();

        turnText.color = new Color(turnText.color.r, turnText.color.g, turnText.color.b, 1);
        DetermineStartingPlayer();
        DisplayPlayerInfo();
    }

    private void DisplayPlayerInfo()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;

        if (playerNameText != null)
        {
            //string playerClass = localPlayer.CustomProperties.ContainsKey("playerClass")
            //    ? localPlayer.CustomProperties["playerClass"].ToString()
            //    : "None";

            playerNameText.text = $"{localPlayer.NickName}";
        }

        if (opponentNameText != null)
        {
            foreach (var p in PhotonNetwork.PlayerList)
            {
                if (!p.IsLocal)
                {
                    //string oppClass = p.CustomProperties.ContainsKey("playerClass")
                    //    ? p.CustomProperties["playerClass"].ToString()
                    //    : "None";

                    opponentNameText.text = $"{p.NickName}";
                    break;
                }
            }
        }
    }

    private void DetermineStartingPlayer()
    {
        isPlayerTurn = PhotonNetwork.IsMasterClient;
        UpdateTurnUI();

        if (isPlayerTurn)
            BeginPlayerTurn();
        else
            turnButton.SetActive(false);
    }

    [PunRPC]
    public void EndTurn()
    {
        if (!isPlayerTurn) return;

        isPlayerTurn = false;
        UpdateTurnUI();
        turnButton.SetActive(false);

        photonView.RPC("BeginPlayerTurn", RpcTarget.Others);
    }

    [PunRPC]
    public void BeginPlayerTurn()
    {
        if (health.currentHealth > 0)
        {
            isPlayerTurn = true;

            playerStats.RestoreBandwidth();
            CardManager cardManager = FindObjectOfType<CardManager>();
            if (cardManager != null)
                cardManager.DrawMultipleCards(4);

            turnButton.SetActive(true);
            UpdateTurnUI();
        }
    }

    private void UpdateTurnUI()
    {
        if (health.currentHealth > 0)
        {
            Debug.Log("Turn Update - isPlayerTurn: " + isPlayerTurn);

            turnText.text = isPlayerTurn ? "Your Turn" : "Opponent's Turn";
            turnText.color = new Color(turnText.color.r, turnText.color.g, turnText.color.b, 1);
            turnText.gameObject.SetActive(true);
            turnText.DOFade(1, 1f);

            Invoke(nameof(HideTurnText), 1);
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
                Invoke(nameof(HideOpponentCardText), 1.2f);
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
