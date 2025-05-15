using UnityEngine;
using TMPro;
using DG.Tweening;
using Unity.Collections.LowLevel.Unsafe;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Player")]
    public bool isPlayerTurn = true;
    public PlayerStats playerStats;
    public Opponent opponent;
    public Health health;

    [Header("Game Objects")]
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI opponentCardText;
    [HideInInspector] public CardManager cardManager;
    public GameObject turnButton;

    public Cards cards;

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
        UpdateTurnText();

        cards = GetComponent<Cards>();
        cards.isSingleplayer = true;
    }

    public void EndPlayerTurn()
    {
        if (!isPlayerTurn) return;

        isPlayerTurn = false;

        Invoke("OpponentTurn", 1);
        UpdateTurnText();
    }


    private void OpponentTurn()
    {
        if (health.currentHealth > 0)
        {
            UpdateTurnText();

            turnButton.SetActive(false);
            opponent.ExecuteTurn();
            opponent.RestoreBandwidth();

            BurnEffectTrigger();
        }
    }

    private void BurnEffectTrigger()
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();

        var target = FindObjectOfType<PlayerStats>();
        if (target != null && target.isBurning)
        {
            playerStats.StartBurnEffect(3, 5);
        }
    }

    public void StartPlayerTurn()
    {
        if (health.currentHealth > 0)
        {
            isPlayerTurn = true;
            playerStats.RestoreBandwidth();

            CardManager cardManager = FindObjectOfType<CardManager>();
            cardManager.DrawMultipleCards(4);

            turnButton.SetActive(true);
            UpdateTurnText();
        }
    }


    private void UpdateTurnText()
    {
        if (health.currentHealth > 0)
        {
            Debug.Log("Turn Update - isPlayerTurn: " + isPlayerTurn);

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
}
