using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PhotonTurnManager : MonoBehaviourPun
{
    public static PhotonTurnManager Instance;

    [Header("Player")]
    public bool isPlayerTurn = true;
    public PlayerStats playerStats;
    public Health health;

    [Header("Game Objects")]
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI opponentCardText;
    public GameObject turnButton;

    [Header("UI - Player Info")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI opponentNameText;

    public Cards cards;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        health = FindObjectOfType<Health>();
        UpdateTurnText();
        DisplayPlayerInfo();

        cards = GetComponent<Cards>();
        cards.isSingleplayer = false;
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

    public void EndPlayerTurn()
    {
        if (!isPlayerTurn) return;

        if (photonView.IsMine)
        {
            isPlayerTurn = false;
            photonView.RPC("RPC_EndPlayerTurn", RpcTarget.Others);
            UpdateTurnText();
            Invoke("OpponentTurn", 1);
        }
    }

    private void OpponentTurn()
    {
        if (!photonView.IsMine)
        {
            if (health.currentHealth > 0)
            {
                UpdateTurnText();
                turnButton.SetActive(false);
                BurnEffectTrigger();
            }

            photonView.RPC("RPC_StartPlayerTurn", RpcTarget.Others);
        }
    }

    public void StartPlayerTurn()
    {
        if (health.currentHealth > 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                isPlayerTurn = true;
                playerStats.RestoreBandwidth();

                CardManager cardManager = FindObjectOfType<CardManager>();
                cardManager.DrawMultipleCards(4);

                turnButton.SetActive(true);
                UpdateTurnText();
            }
        }
    }

    private void BurnEffectTrigger()
    {
        if (playerStats != null && playerStats.isBurning)
        {
            playerStats.StartBurnEffect(3, 5);
        }
    }

    private void UpdateTurnText()
    {
        if (health.currentHealth > 0)
        {
            turnText.text = isPlayerTurn ? "Player's Turn" : "Opponent's Turn";
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

    public void DisplayPlayedCard(string playerName, string cardName)
    {
        if (health.currentHealth > 0)
        {
            if (playerName == "Opponent" && opponentCardText != null)
            {
                opponentCardText.text = "Opponent played: " + cardName;
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

    [PunRPC]
    private void RPC_EndPlayerTurn()
    {
        isPlayerTurn = false;
        UpdateTurnText();
        Invoke(nameof(OpponentTurn), 1);
    }

    [PunRPC]
    private void RPC_StartPlayerTurn()
    {
        isPlayerTurn = true;
        StartPlayerTurn();
    }
}
