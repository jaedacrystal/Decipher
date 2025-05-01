using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class PlayerStats : MonoBehaviourPunCallbacks
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

    public CardManager cardManager;

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
            GetComponent<Health>()?.TakeDamage(burnEffectDamage);
            yield return new WaitForSeconds(1f);
        }
        isBurning = false;
    }

    public bool CanPlayCard(int cardBandwidth)
    {
        Debug.Log($"Checking bandwidth: Current: {currentBandwidth}, Required: {cardBandwidth}");
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
    public void RPC_RestoreBandwidth(int restoredBandwidth)
    {
        if (!isPlayer) return;
        currentBandwidth = restoredBandwidth;

        UpdateBandwidth();

        DOTween.To(() => currentBandwidth, x =>
        {
            bandwidthText.text = $"{x}/{maxBandwidth}";
        }, currentBandwidth, 0.5f).SetEase(Ease.OutQuad);

        int iconIndex = Mathf.Clamp(5 - currentBandwidth, 0, bandwidthIconArray.Length - 1);
        bandwidthIcon.GetComponent<Image>().sprite = bandwidthIconArray[iconIndex];

        Debug.Log("Bandwidth restored remotely: " + currentBandwidth);
    }

    public void PlayCard(Cards card)
    {
        UseBandwidth(card.bandwidth);

        if (GameManager.Instance.IsSingleplayer)
        {
            GameObject player = TurnManager.Instance.playerPrefab;
            GameObject opponent = TurnManager.Instance.opponentPrefab;

            card.ApplyEffect(player, opponent);
            TurnManager.Instance.DisplayPlayedCard("Player", card.cardName);
        }
        else
        {
            photonView.RPC("RPC_PlayCard", RpcTarget.All, card.cardName, card.bandwidth, PhotonNetwork.LocalPlayer.ActorNumber);
            MultiTurnManager.Instance.DisplayPlayedCard(PhotonNetwork.NickName, card.cardName);
        }
    }

    [PunRPC]
    public void RPC_PlayCard(string cardName, int bandwidth, int actorNumber)
    {
        Debug.Log($"[RPC] Playing card '{cardName}' by actor {actorNumber}");

        GameObject myPlayer = MultiTurnManager.Instance.myPlayer;
        GameObject opponent = MultiTurnManager.Instance.opponent;

        GameObject caster = (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber) ? myPlayer : opponent;
        GameObject target = (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber) ? opponent : myPlayer;

        if (caster == null || target == null)
        {
            Debug.LogError("Caster or target is null in RPC_PlayCard.");
            return;
        }

        Cards card = FindCardByName(cardName);
        if (card == null)
        {
            Debug.LogError($"Card '{cardName}' not found in MultiCardManager.");
            return;
        }

        Debug.Log($"Applying effect of card '{card.cardName}' from {caster.name} to {target.name}");
        card.ApplyEffect(caster, target);
    }

    private Cards FindCardByName(string name)
    {
        MultiCardManager multiCardManager = FindObjectOfType<MultiCardManager>();
        if (multiCardManager == null)
        {
            Debug.LogError("MultiCardManager not found.");
            return null;
        }

        foreach (Cards card in multiCardManager.listOfCards)
        {
            if (card.cardName == name)
                return card;
        }

        Debug.LogError($"Card '{name}' not found in list.");
        return null;
    }

    private void UpdateBandwidth()
    {
        bandwidthText.text = $"{currentBandwidth}/{maxBandwidth}";

        int iconIndex = Mathf.Clamp(5 - currentBandwidth, 0, bandwidthIconArray.Length - 1);
        bandwidthIcon.GetComponent<Image>().sprite = bandwidthIconArray[iconIndex];
    }
}
