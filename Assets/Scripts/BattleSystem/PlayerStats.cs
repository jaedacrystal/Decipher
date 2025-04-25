using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerStats : MonoBehaviour
{
    public int maxBandwidth = 5;
    public int currentBandwidth;
    public int defense = 0;
    public bool isInvulnerable = false;
    public TextMeshProUGUI bandwidthText;
    public bool isPlayer = true;

    public Sprite[] bandwidthIconArray;
    public GameObject bandwidthIcon;

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

    public void IncreaseDefenseAndDebuff(int amount)
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

        if (isPlayer)
        {
            UpdateBandwidth();

            DOTween.To(() => previousBandwidth, x =>
            {
                bandwidthText.text = $"{x}/{maxBandwidth}";
            }, currentBandwidth, 0.5f).SetEase(Ease.OutQuad);
        }
    }

    public void RestoreBandwidth()
    {
        int previousBandwidth = currentBandwidth;
        currentBandwidth = maxBandwidth;

        if(isPlayer)
        {
            UpdateBandwidth();

            DOTween.To(() => previousBandwidth, x =>
            {
                bandwidthText.text = $"{x}/{maxBandwidth}";
            }, currentBandwidth, 0.5f).SetEase(Ease.OutQuad);
        }
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

        if (currentBandwidth == 5)
        {
            bandwidthIcon.GetComponent<Image>().sprite = bandwidthIconArray[0];
        } else if (currentBandwidth == 4)
        {
            bandwidthIcon.GetComponent<Image>().sprite = bandwidthIconArray[1];
        } else if (currentBandwidth == 3)
        {
            bandwidthIcon.GetComponent<Image>().sprite = bandwidthIconArray[2];
        } else if (currentBandwidth == 2)
        {
            bandwidthIcon.GetComponent<Image>().sprite = bandwidthIconArray[3];
        } else if (currentBandwidth == 1)
        {
            bandwidthIcon.GetComponent<Image>().sprite = bandwidthIconArray[4];
        } else if (currentBandwidth == 0)
        {
            bandwidthIcon.GetComponent<Image>().sprite = bandwidthIconArray[5];
        }
    }
}
