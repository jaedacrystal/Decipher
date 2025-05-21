using System.Collections;
using System.ComponentModel;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Cards : ScriptableObject
{
    public Sprite artwork;
    public string cardName;

    [TextArea(3, 3)] public string desc;
    [TextArea(3, 3)] public string flavorTxt;

    public int bandwidth;
    public int effectValue;
    public int effectRounds;

    [Header("Class Effects")]
    public ClassType classType;
    public EffectType effectType;
    public TargetType target;

    public bool isSingleplayer = true;
    public TextMeshProUGUI cardText;

    public CardManager cardManager;
    public PhotonCardManager photonCardManager;

    TurnManager turnManager;
    PhotonTurnManager photonTurnManager;

    public void ApplySingleplayerEffect ( PlayerStats playerStats, Health playerHealth, PlayerStats opponentStats, Health opponentHealth, TargetType target ) {
        Debug.Log ( $"PlayerHealth: {playerHealth}, PlayerStats: {playerStats}" );
        Debug.Log ( $"OpponentHealth: {opponentHealth}, OpponentStats: {opponentStats}" );

        Health targetHealth = target == TargetType.Player ? playerHealth : opponentHealth;
        PlayerStats targetStats = target == TargetType.Player ? playerStats : opponentStats;

        if ( targetHealth == null || targetStats == null ) {
            Debug.LogError ( "Target is missing required components!" );
            return;
        }

        ApplyEffectLogic ( targetHealth, targetStats );
    }

    // MULTIPLAYER MODE

    [PunRPC]
    public void ApplyEffectMulti ( PlayerStats playerStats, Health playerHealth, PlayerStats opponentStats, Health opponentHealth, GameObject target ) {

        Health attackHealth = playerHealth;
        PlayerStats attackingStats = playerStats;

        Health targetHealth = opponentHealth;
        PlayerStats targetStats = opponentStats;

        ApplyEffectLogic ( targetHealth, targetStats );
    }

    // EFFECT LOGIC
    private void ApplyEffectLogic(Health targetHealth, PlayerStats targetStats)
    {
        switch (effectType)
        {
            case EffectType.Attack:
                targetHealth.PublicTakeDamage ( effectValue );
                break;

            case EffectType.Defense:
                targetStats.IncreaseDefense(effectValue);
                break;

            case EffectType.StrongPassword:
                targetHealth.Buff();
                ApplyStrongPassword(targetStats);
                break;

            case EffectType.Heal:
                HealSelf(targetHealth);
                break;

            case EffectType.ShieldAndRetaliate:
                targetHealth.Buff();

                TurnManager turnManager = TurnManager.Instance;
                if (turnManager != null)
                {
                    turnManager.StartCoroutine(ApplyShieldAndRetaliate(targetHealth, targetStats, effectValue));
                } else {
                    PhotonTurnManager photonTurnManager = PhotonTurnManager.Instance;
                    if (photonTurnManager != null)
                    {
                        photonTurnManager.StartCoroutine(ApplyShieldAndRetaliate(targetHealth, targetStats, effectValue));
                    }
                }
                break;

            case EffectType.Burn:
                StartBurnEffect(targetHealth, effectValue);
                break;

            case EffectType.DataLeak:
                targetHealth.Debuff();

                cardManager = FindObjectOfType<CardManager>();

                if(cardManager == null)
                {
                    photonCardManager = FindObjectOfType<PhotonCardManager>();
                    PlayerStats playerStats = targetStats;

                    PhotonView photonView = targetStats.GetComponent<PhotonView>();
                    photonView.RPC("RPC_DataLeak", RpcTarget.All);
                } else
                {
                    ApplyDataLeak(targetStats);
                }
                break;

            case EffectType.ZeroDayExploit:
                ApplyZeroDayExploit(targetHealth, targetStats, targetStats);
                break;

            case EffectType.ForgetPassword:
                ApplyForgetPasswordFromGraveyard();
                break;

            case EffectType.RansomwareAttack:
                targetHealth.Debuff();
                ApplyRansomwareAttack();
                break;
        }
    }

    // SPECIAL EFFECT METHODS

    public void HealSelf(Health targetHealth)
    {
        cardManager = FindObjectOfType<CardManager>();

        targetHealth = cardManager.playerHealth;

        if (cardManager == null)
        {
            photonCardManager = FindObjectOfType<PhotonCardManager>();

            if (photonCardManager.player1)
            {
                targetHealth = photonCardManager.player1Health;
            }
            else
            {
                targetHealth = photonCardManager.player2Health;
            }
        }

        targetHealth.PublicHeal(effectValue);
    }

    private void ApplyForgetPasswordFromGraveyard()
    {
        CardManager cardManager = FindObjectOfType<CardManager>();

        if (cardManager == null)
        {
            PhotonCardManager photonCardManager = FindObjectOfType<PhotonCardManager>();
        }

        Discard discard = FindObjectOfType<Discard>();
        if (discard == null) return;

        Transform graveyardTransform = discard.graveyard.transform;
        int graveyardCount = graveyardTransform.childCount;
        if (graveyardCount == 0) return;

        Health playerHealth = cardManager.playerHealth;
        if (playerHealth != null)
            playerHealth.PublicTakeDamage(2);

        int randomIndex = Random.Range(0, graveyardCount);
        Transform cardTransform = graveyardTransform.GetChild(randomIndex);

        cardTransform.SetParent(cardManager.hand.transform, false);
        cardTransform.gameObject.SetActive(true);
        cardTransform.localScale = Vector3.zero;
        cardTransform.DOScale(Vector3.one, 0.3f);

        cardManager.CardPosition();
        discard.UpdateGraveyardCounter();

        cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
        cardText.text = "Forget Password retrieved: " + cardTransform.name;
        cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1);
        cardText.gameObject.SetActive(true);
        cardText.DOFade(1, 1f).OnComplete(() => cardText.DOFade(0, 1f));
    }

    private void ApplyRansomwareAttack()
    {
        CardManager cardManager = FindObjectOfType<CardManager>();

        if (cardManager != null)
        {
            cardManager.DiscardCardFromOpponentDeck();
            cardManager.DiscardCardFromOpponentDeck();
        } else
        {
            photonCardManager = FindObjectOfType<PhotonCardManager>();

            if (photonCardManager.player1)
            {
                PhotonView photonView = photonCardManager.GetComponent<PhotonView>();
                photonView.RPC("RPC_Discard", RpcTarget.Others);
                photonView.RPC("RPC_Discard", RpcTarget.Others);
            }
        }

        cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
        cardText.text = "Opponent has discarded 2 cards";
        cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1);
        cardText.gameObject.SetActive(true);
        cardText.DOFade(1, 1f).OnComplete(() => cardText.DOFade(0, 1f));
    }

    //private void ApplyStrongPassword(PlayerStats targetStats)
    //{
    //    targetStats.isProtected = true;
    //    targetStats.damageTakenMultiplier *= 0.5f;

    //    cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
    //    cardText.text = "Strong Password applied!";
    //    cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1);
    //    cardText.gameObject.SetActive(true);
    //    cardText.DOFade(1, 1f).OnComplete(() => cardText.DOFade(0, 1f));

    //    turnManager = FindObjectOfType<TurnManager>();
    //    photonTurnManager = FindObjectOfType<PhotonTurnManager>();

    //    if (turnManager != null) {

    //        TurnManager turnManager = TurnManager.Instance;
    //        if (turnManager != null)
    //        {
    //            turnManager.StartCoroutine(RemoveStrongPasswordAfterTurn(targetStats));
    //        }
    //        else
    //        {
    //            PhotonTurnManager photonTurnManager = PhotonTurnManager.Instance;
    //            if (photonTurnManager != null)
    //            {
    //                photonTurnManager.StartCoroutine(RemoveStrongPasswordAfterTurn(targetStats));
    //            }
    //        }
    //    }
    //}

    private void ApplyStrongPassword(PlayerStats targetStats)
    {
        PhotonView photonView = targetStats.GetComponent<PhotonView>();

        // UI Feedback
        cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
        cardText.text = "Strong Password applied!";
        cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1);
        cardText.gameObject.SetActive(true);
        cardText.DOFade(1, 1f).OnComplete(() => cardText.DOFade(0, 1f));

        // Coroutine
        turnManager = FindObjectOfType<TurnManager>();
        cardManager = FindObjectOfType<CardManager>();

        if (turnManager == null && cardManager == null) {
            photonTurnManager = FindObjectOfType<PhotonTurnManager>();
            photonCardManager = FindObjectOfType<PhotonCardManager>();
        }

        if (turnManager != null)
        {
            targetStats = turnManager.playerStats;
            targetStats.isProtected = true;
            targetStats.damageTakenMultiplier *= 0.5f;
            turnManager.StartCoroutine(RemoveStrongPasswordAfterTurn(targetStats));
        }
        else if (photonTurnManager != null)
        {
            if (photonCardManager.player1)
            {
                targetStats = photonCardManager.player1Stats;
            }
            else
            {
                targetStats = photonCardManager.player2Stats;
            }
            photonView.RPC("SetStrongPassword", RpcTarget.All, true);
            photonTurnManager.StartCoroutine(RemoveStrongPasswordAfterTurn(targetStats));
        }
    }


    private IEnumerator RemoveStrongPasswordAfterTurn(PlayerStats targetStats)
    {
        if (turnManager != null)
        {
            yield return new WaitUntil(() => !TurnManager.Instance.isPlayerTurn);
            yield return new WaitUntil(() => TurnManager.Instance.isPlayerTurn);
        } else {
            if (photonCardManager.player1)
            {
                targetStats = photonCardManager.player1Stats;
            }
            else
            {
                targetStats = photonCardManager.player2Stats;
            }
            yield return new WaitUntil(() => !PhotonTurnManager.Instance.isPlayerTurn);
            yield return new WaitUntil(() => PhotonTurnManager.Instance.isPlayerTurn);
        }

        targetStats.isProtected = false;
        targetStats.damageTakenMultiplier = 1f;
    }

    private void StartBurnEffect(Health targetHealth, int effectValue)
    {
        PlayerStats stats = targetHealth.GetComponent<PlayerStats>();
        if (stats != null)
            stats.StartBurnEffect(effectRounds, effectValue);
    }

    private void ApplyDataLeak(PlayerStats stats)
    {
        stats.damageTakenMultiplier *= 1.2f;

        cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
        cardText.text = "Opponent is now vulnerable to 20% more damage!";
        cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1);
        cardText.gameObject.SetActive(true);
        cardText.DOFade(1, 1f).OnComplete(() => cardText.DOFade(0, 1f));
    }

    private void ApplyZeroDayExploit(Health targetHealth, PlayerStats targetStats, PlayerStats opponentStats)
    {
        int damage = targetStats.isProtected ? effectValue : effectValue * 2;
        targetHealth.PublicTakeDamage(damage);

        cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
        cardText.text = $"Opponent took {damage} damage!";
        cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1);
        cardText.gameObject.SetActive(true);
        cardText.DOFade(1, 1f).OnComplete(() => cardText.DOFade(0, 1f));
    }

    private IEnumerator ApplyShieldAndRetaliate(Health casterHealth, PlayerStats casterStats, int effectValue)
    {
        cardManager = FindObjectOfType<CardManager>();

        if (cardManager == null)
        {
            photonCardManager = FindObjectOfType<PhotonCardManager>();
        }

        Health opponentHealth;
        PlayerStats playerToMakeInvulnerable;

        if (cardManager != null)
        {
            TurnManager turnManager = TurnManager.Instance;

            opponentHealth = cardManager.opponentHealth;
            playerToMakeInvulnerable = cardManager.playerStats;

            opponentHealth.PublicTakeDamage(effectValue);

            yield return new WaitUntil(() => !turnManager.isPlayerTurn);

            playerToMakeInvulnerable.isInvulnerable = true;
        }
        else
        {
            PhotonTurnManager turnManager = PhotonTurnManager.Instance;

            if (photonCardManager.player1)
            {
                opponentHealth = photonCardManager.player2Health;
                playerToMakeInvulnerable = photonCardManager.player1Stats;
            }
            else
            {
                opponentHealth = photonCardManager.player1Health;
                playerToMakeInvulnerable = photonCardManager.player2Stats;
            }

            opponentHealth.PublicTakeDamage(effectValue);

            yield return new WaitUntil(() => !turnManager.isPlayerTurn);

            PhotonView photonView = playerToMakeInvulnerable.GetComponent<PhotonView>();

            photonView.RPC("SetInvulnerability", RpcTarget.All, true);
        }

        cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
        cardText.text = "You are now invulnerable!";
        cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1);
        cardText.gameObject.SetActive(true);
        cardText.DOFade(1, 1f).OnComplete(() => cardText.DOFade(0, 1f));

        yield return new WaitUntil(() =>
            cardManager != null
                ? TurnManager.Instance.isPlayerTurn
                : PhotonTurnManager.Instance.isPlayerTurn);

        if (cardManager != null)
        {
            playerToMakeInvulnerable.isInvulnerable = false;
        }
        else
        {
            PhotonView photonView = playerToMakeInvulnerable.GetComponent<PhotonView>();
            photonView.RPC("SetInvulnerability", RpcTarget.All, false);
        }
    }


    //private IEnumerator ApplyShieldAndRetaliate(Health casterHealth, PlayerStats casterStats, int effectValue)
    //{
    //    cardManager = FindObjectOfType<CardManager>();

    //    if (cardManager == null)
    //    {
    //        photonCardManager = FindObjectOfType<PhotonCardManager>();
    //    }

    //    Health opponentHealth;
    //    PlayerStats playerToMakeInvulnerable;

    //    if (cardManager != null)
    //    {
    //        // Singleplayer
    //        TurnManager turnManager = TurnManager.Instance;

    //        opponentHealth = cardManager.opponentHealth;    // damage opponent
    //        playerToMakeInvulnerable = cardManager.playerStats; // invulnerable = caster stats

    //        opponentHealth.PublicTakeDamage(effectValue);

    //        yield return new WaitUntil(() => !turnManager.isPlayerTurn);
    //    }
    //    else
    //    {
    //        // Multiplayer
    //        PhotonTurnManager turnManager = PhotonTurnManager.Instance;

    //        if (photonCardManager.player1)
    //        {
    //            opponentHealth = photonCardManager.player2Health; // damage opponent
    //            playerToMakeInvulnerable = photonCardManager.player1Stats; // caster invulnerable
    //        }
    //        else
    //        {
    //            opponentHealth = photonCardManager.player1Health;
    //            playerToMakeInvulnerable = photonCardManager.player2Stats;
    //        }
    //        PhotonView photonView = photonCardManager.GetComponent<PhotonView>();

    //        opponentHealth.PublicTakeDamage(effectValue);

    //        yield return new WaitUntil(() => !turnManager.isPlayerTurn);
    //    }

    //    // Apply invulnerability to caster
    //    playerToMakeInvulnerable.isInvulnerable = true;

    //    cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
    //    cardText.text = "You are now invulnerable!";
    //    cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1);
    //    cardText.gameObject.SetActive(true);
    //    cardText.DOFade(1, 1f).OnComplete(() => cardText.DOFade(0, 1f));

    //    // Wait until next turn starts
    //    yield return new WaitUntil(() =>
    //        cardManager != null
    //            ? TurnManager.Instance.isPlayerTurn
    //            : PhotonTurnManager.Instance.isPlayerTurn);

    //    playerToMakeInvulnerable.isInvulnerable = false;
    //}
}

public enum EffectType
{
    None,
    Attack,
    Defense,
    Heal,
    Draw,
    ShieldAndRetaliate,
    Burn,
    DataLeak,
    ZeroDayExploit,
    StrongPassword,
    ForgetPassword,
    RansomwareAttack
}

public enum ClassType
{
    Defense,
    Offense,
    Balance
}

public enum TargetType
{
    Player,
    Opponent
}
