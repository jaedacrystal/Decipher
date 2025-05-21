using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using Photon.Pun;

public class PlayerStats : MonoBehaviour
{
    public int maxBandwidth = 5;
    public int currentBandwidth;
    public int defense = 0;
    public bool isInvulnerable = false;
    public bool isProtected = false;
    public float damageTakenMultiplier = 1f;
    public bool isStrongPasswordActive = false;
    public TextMeshProUGUI bandwidthText;
    public bool isPlayer = true;

    public Sprite[] bandwidthIconArray;
    public GameObject bandwidthIcon;

    public PhotonCardManager cardManager;

    public bool isBurning = false;
    public int burnEffectRounds = 0;
    public int burnEffectDamage = 0;

    private void Start()
    {
        currentBandwidth = maxBandwidth;
        UpdateBandwidth();
    }

    public void IncreaseDefense(int amount)
    {
        defense += amount;
    }

    public void StartBurnEffect(int effectRounds, int effectDamage)
    {
        isBurning = true;
        burnEffectRounds = effectRounds;
        burnEffectDamage = effectDamage;
        StartCoroutine(ApplyBurnEffect());
    }

    private IEnumerator ApplyBurnEffect()
    {
        for (int i = 0; i < burnEffectRounds; i++)
        {
            GetComponent<Health>()?.PublicTakeDamage(burnEffectDamage);
            yield return new WaitForSeconds(1f);
        }
        isBurning = false;
    }

    public bool CanPlayCard(int cardBandwidth)
    {
        return currentBandwidth >= cardBandwidth;
    }

    public void UseBandwidth(int amount)
    {
        int previousBandwidth = currentBandwidth;
        currentBandwidth -= amount;
        currentBandwidth = Mathf.Max(0, currentBandwidth);

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

        if (isPlayer)
        {
            UpdateBandwidth();

            DOTween.To(() => previousBandwidth, x =>
            {
                bandwidthText.text = $"{x}/{maxBandwidth}";
            }, currentBandwidth, 0.5f).SetEase(Ease.OutQuad);
        }
    }

    [PunRPC]
    public void SetInvulnerability(bool value)
    {
        isInvulnerable = value;
    }

    [PunRPC]
    public void SetStrongPassword(bool isActive)
    {
        isProtected = isActive;
        damageTakenMultiplier = isActive ? damageTakenMultiplier * 0.5f : damageTakenMultiplier * 2f;
        Debug.Log($"{gameObject.name} strong password status set to {isActive}, new multiplier: {damageTakenMultiplier}");
    }


    //public void PlayCard(Cards card)
    //{
    //    if (CanPlayCard(card.bandwidth))
    //    {
    //        UseBandwidth(card.bandwidth);
    //        card.ApplyEffect(gameObject, TurnManager.Instance.opponent.gameObject);

    //        TurnManager.Instance.DisplayPlayedCard("Player", card.cardName);
    //    }
    //}

    private void UpdateBandwidth()
    {
        bandwidthText.text = $"{currentBandwidth}/{maxBandwidth}";

        int iconIndex = Mathf.Clamp(5 - currentBandwidth, 0, bandwidthIconArray.Length - 1);
        bandwidthIcon.GetComponent<Image>().sprite = bandwidthIconArray[iconIndex];
    }
}
