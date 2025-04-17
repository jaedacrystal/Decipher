using UnityEngine;
using TMPro;
using DG.Tweening;

public class PlayerStats : MonoBehaviour
{
    public int maxBandwidth = 10;
    public int currentBandwidth;
    public int defense = 0;
    public bool isInvulnerable = false;
    public TextMeshProUGUI bandwidthText;
    public bool isPlayer = true;

    public CardManager cardManager;

    private void Start()
    {
        currentBandwidth = maxBandwidth;
        UpdateBandwidth();
    }

    public void IncreaseDefense(int amount)
    {
        defense += amount;
    }

    public bool CanPlayCard(int cardBandwidth)
    {
        return currentBandwidth >= cardBandwidth;
    }

    public void UseBandwidth(int amount)
    {
        int previousBandwidth = currentBandwidth;
        currentBandwidth -= amount;
        if (currentBandwidth < 0) currentBandwidth = 0;

        DOTween.To(() => previousBandwidth, x =>
        {
            bandwidthText.text = $"{x}/{maxBandwidth}";
        }, currentBandwidth, 0.5f).SetEase(Ease.OutQuad);
    }

    public void RestoreBandwidth()
    {
        int previousBandwidth = currentBandwidth;
        currentBandwidth = maxBandwidth;

        DOTween.To(() => previousBandwidth, x =>
        {
            bandwidthText.text = $"{x}/{maxBandwidth}";
        }, currentBandwidth, 0.5f).SetEase(Ease.OutQuad);
    }

    public void PlayCard(Cards card)
    {
        if (CanPlayCard(card.bandwidth))
        {
            UseBandwidth(card.bandwidth);
            card.ApplyEffect(gameObject, TurnManager.Instance.opponent.gameObject);

            TurnManager.Instance.DisplayPlayedCard("Player", card.cardName);
        }
    }

    private void UpdateBandwidth()
    {
        bandwidthText.text = $"{currentBandwidth}/{maxBandwidth}";
    }
}
