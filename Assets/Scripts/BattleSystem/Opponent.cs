using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Opponent : MonoBehaviour
{
    public PlayerStats opponentStats;
    public PlayerStats playerStats;
    public CardManager cardManager;

    private void Awake()
    {
        opponentStats.isPlayer = false;
    }

    public void ExecuteTurn()
    {
        StartCoroutine(PerformActions());
    }

    private IEnumerator PerformActions()
    {
        yield return new WaitForSeconds(1);

        List<Cards> playableCards = GetPlayableCards();

        while (playableCards.Count > 0)
        {
            Cards chosenCard = playableCards[Random.Range(0, playableCards.Count)];

            opponentStats.UseBandwidth(chosenCard.bandwidth);
            chosenCard.ApplyEffect(playerStats.gameObject, opponentStats.gameObject);

            TurnManager.Instance.DisplayPlayedCard("Opponent", chosenCard.cardName);

            yield return new WaitForSeconds(1);
            playableCards = GetPlayableCards();
        }
        TurnManager.Instance.StartPlayerTurn();
    }

    public void RestoreBandwidth()
    {
        opponentStats.RestoreBandwidth();
    }

    private List<Cards> GetPlayableCards()
    {
        List<Cards> playableCards = new List<Cards>();

        foreach (Cards card in cardManager.opponentDeck)
        {
            if (opponentStats.CanPlayCard(card.bandwidth))
            {
                playableCards.Add(card);
            }
        }
        return playableCards;
    }
}
