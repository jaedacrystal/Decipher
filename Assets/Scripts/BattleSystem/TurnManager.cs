using UnityEngine;
using TMPro;
using DG.Tweening;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public bool isPlayerTurn = true;
    public PlayerStats playerStats;
    public Opponent opponent;

    public TextMeshProUGUI turnText;
    public TextMeshProUGUI opponentCardText;
    public CardManager cardManager;

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
        turnText.color = new Color(turnText.color.r, turnText.color.g, turnText.color.b, 1);
        UpdateTurnText();
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
        UpdateTurnText();

        opponent.ExecuteTurn();
        opponent.RestoreBandwidth();
    }


    public void StartPlayerTurn()
    {
        isPlayerTurn = true;
        playerStats.RestoreBandwidth();

        CardManager cardManager = FindObjectOfType<CardManager>();
        cardManager.DrawMultipleCards(4);

        UpdateTurnText();
    }


    private void UpdateTurnText()
    {
        Debug.Log("Turn Update - isPlayerTurn: " + isPlayerTurn);

        turnText.text = isPlayerTurn ? "Player's Turn" : "Opponent's Turn";
        turnText.color = new Color(turnText.color.r, turnText.color.g, turnText.color.b, 1);
        turnText.gameObject.SetActive(true);
        turnText.DOFade(1, 0.2f);

        Invoke("HideTurnText", 2);
    }


    private void HideTurnText()
    {
        turnText.DOFade(0, 0.5f).OnComplete(() => turnText.gameObject.SetActive(false));
    }

    public void DisplayPlayedCard(string playerName, string cardName)
    {
        if (playerName == "Opponent" && opponentCardText != null)
        {
            opponentCardText.text = "Opponent played: " + cardName;
            AnimateText(opponentCardText);
            Invoke("HideOpponentCardText", 3);
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
