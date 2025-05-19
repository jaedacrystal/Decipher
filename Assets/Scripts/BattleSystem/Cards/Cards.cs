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

    private void Start()
    {
        isSingleplayer = GameManager.Instance.IsSingleplayer;
    }

    //public void ApplyEffect(GameObject player, GameObject opponent)
    //{
    //    if (GameManager.Instance.IsSingleplayer == true)
    //        ApplySingleplayerEffect(player, opponent);
    //    else
    //        ApplyMultiplayerEffect(player, opponent);
    //}

    //public void ApplyEffect ( PlayerStats playerStats, Health playerHealth, PlayerStats opponentStats, Health opponentHealth, TargetType target ) {

    //    if ( GameManager.Instance.IsSingleplayer == true )
    //        ApplySingleplayerEffect ( playerStats, playerHealth, opponentStats, opponentHealth, target );
    //    //else
    //        //ApplyMultiplayerEffect ( player, opponent );
    //}

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


    //public void ApplySingleplayerEffect ( GameObject player, GameObject opponent ) {
    //    Health playerHealth = player.GetComponent<Health> ();
    //    PlayerStats playerStats = player.GetComponent<PlayerStats> ();
    //    Health opponentHealth = opponent.GetComponent<Health> ();
    //    PlayerStats opponentStats = opponent.GetComponent<PlayerStats> ();

    //    GameObject targetObject = target == TargetType.Player ? player : opponent;
    //    Health targetHealth = targetObject.GetComponent<Health> ();
    //    PlayerStats targetStats = targetObject.GetComponent<PlayerStats> ();

    //    if ( targetHealth == null || targetStats == null ) {
    //        Debug.LogError ( "Target (player or opponent) is missing required components in singleplayer mode!" );
    //        return;
    //    }

    //    ApplyEffectLogic ( targetHealth, targetStats );
    //}

    // MULTIPLAYER MODE

    public void ApplyEffectMulti ( PlayerStats playerStats, Health playerHealth, PlayerStats opponentStats, Health opponentHealth, GameObject target ) {
        //if ( localPlayer == null || remotePlayer == null ) {
        //    Debug.LogError ( "Local or remote player is null in multiplayer mode." );
        //    return;
        //}

        Health attackHealth = playerHealth;
        PlayerStats attackingStats = playerStats;

        Health targetHealth = opponentHealth;
        PlayerStats targetStats = opponentStats;

        //if ( localHealth == null || localStats == null || remoteHealth == null || remoteStats == null ) {
        //    Debug.LogError ( "Missing Health or PlayerStats on one of the players in multiplayer mode." );
        //    return;
        //}

        //PhotonCardManager photonCardManager = FindAnyObjectByType<PhotonCardManager> ();
        //TurnManager turnManager = FindAnyObjectByType<TurnManager> ();

        //GameObject targetObject = target == TargetType.Player ? localPlayer : remotePlayer;


        //if ( photonCardManager.player1 && turnManager.isPlayerTurn ) {
        //    GameObject targetObject = remotePlayer;
        //} else {
        //    GameObject targetObject = localPlayer;
        //}

        ApplyEffectLogic ( targetHealth, targetStats );
    }

    // ORIGINAL
    //public void ApplyMultiplayerEffect(GameObject localPlayer, GameObject remotePlayer)
    //{
    //    if (localPlayer == null || remotePlayer == null)
    //    {
    //        Debug.LogError("Local or remote player is null in multiplayer mode.");
    //        return;
    //    }

    //    Health localHealth = localPlayer.GetComponent<Health>();
    //    PlayerStats localStats = localPlayer.GetComponent<PlayerStats>();
    //    Health remoteHealth = remotePlayer.GetComponent<Health>();
    //    PlayerStats remoteStats = remotePlayer.GetComponent<PlayerStats>();

    //    if (localHealth == null || localStats == null || remoteHealth == null || remoteStats == null)
    //    {
    //        Debug.LogError("Missing Health or PlayerStats on one of the players in multiplayer mode.");
    //        return;
    //    }

    //    //PhotonCardManager photonCardManager = FindAnyObjectByType<PhotonCardManager> ();
    //    //TurnManager turnManager = FindAnyObjectByType<TurnManager> ();

    //    GameObject targetObject = target == TargetType.Player ? localPlayer : remotePlayer;


    //    //if ( photonCardManager.player1 && turnManager.isPlayerTurn ) {
    //    //    GameObject targetObject = remotePlayer;
    //    //} else {
    //    //    GameObject targetObject = localPlayer;
    //    //}


    //    Health targetHealth = targetObject.GetComponent<Health>();
    //    PlayerStats targetStats = targetObject.GetComponent<PlayerStats>();

    //    ApplyEffectLogic(targetHealth, targetStats);
    //}

    // EFFECT LOGIC
    private void ApplyEffectLogic(Health targetHealth, PlayerStats targetStats)
    {
        switch (effectType)
        {
            case EffectType.Attack:
                targetHealth.TakeDamage ( effectValue );
                break;

            case EffectType.Defense:
                targetStats.IncreaseDefense(effectValue);
                break;

            case EffectType.StrongPassword:
                targetHealth.Buff();
                ApplyStrongPassword(targetStats);
                break;

            case EffectType.Heal:
                targetHealth.Heal(effectValue);
                break;

            case EffectType.Draw:
                CardManager cardManager = FindObjectOfType<CardManager>();
                if (cardManager != null)
                    cardManager.DrawMultipleCards(effectValue);
                break;

            case EffectType.ShieldAndRetaliate:
                TurnManager turnManager = TurnManager.Instance;
                if (turnManager != null)
                    turnManager.StartCoroutine(ApplyShieldAndRetaliate(targetHealth, targetStats, effectValue));
                break;

            case EffectType.Burn:
                StartBurnEffect(targetHealth, effectValue);
                break;

            case EffectType.DataLeak:
                targetHealth.Debuff();
                ApplyDataLeak(targetStats);
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

    private void ApplyForgetPasswordFromGraveyard()
    {
        CardManager cardManager = FindObjectOfType<CardManager>();
        Discard discard = FindObjectOfType<Discard>();
        if (cardManager == null || discard == null) return;

        Transform graveyardTransform = discard.graveyard.transform;
        int graveyardCount = graveyardTransform.childCount;
        if (graveyardCount == 0) return;

        //Health playerHealth = cardManager.player.GetComponent<Health>();
        Health playerHealth = cardManager.playerHealth;
        if (playerHealth != null)
            playerHealth.TakeDamage(5);

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

        cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
        cardText.text = "Opponent has discarded 2 cards";
        cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1);
        cardText.gameObject.SetActive(true);
        cardText.DOFade(1, 1f).OnComplete(() => cardText.DOFade(0, 1f));

        cardManager.DiscardCardFromOpponentDeck();
        cardManager.DiscardCardFromOpponentDeck();
    }

    private void ApplyStrongPassword(PlayerStats targetStats)
    {
        targetStats.isProtected = true;
        targetStats.damageTakenMultiplier *= 0.5f;

        cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
        cardText.text = "Strong Password applied!";
        cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1);
        cardText.gameObject.SetActive(true);
        cardText.DOFade(1, 1f).OnComplete(() => cardText.DOFade(0, 1f));

        TurnManager.Instance.StartCoroutine(RemoveStrongPasswordAfterTurn(targetStats));
    }

    private IEnumerator RemoveStrongPasswordAfterTurn(PlayerStats targetStats)
    {
        yield return new WaitUntil(() => !TurnManager.Instance.isPlayerTurn);
        yield return new WaitUntil(() => TurnManager.Instance.isPlayerTurn);

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
        targetHealth.TakeDamage(damage);

        cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
        cardText.text = $"Opponent took {damage} damage!";
        cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1);
        cardText.gameObject.SetActive(true);
        cardText.DOFade(1, 1f).OnComplete(() => cardText.DOFade(0, 1f));
    }

    private IEnumerator ApplyShieldAndRetaliate(Health playerHealth, PlayerStats opponentStats, int effectValue)
    {
        TurnManager turnManager = TurnManager.Instance;

        playerHealth.TakeDamage(effectValue);
        yield return new WaitUntil(() => !turnManager.isPlayerTurn);

        opponentStats.isInvulnerable = true;

        cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
        cardText.text = "You are now invulnerable!";
        cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1);
        cardText.gameObject.SetActive(true);
        cardText.DOFade(1, 1f).OnComplete(() => cardText.DOFade(0, 1f));

        yield return new WaitUntil(() => turnManager.isPlayerTurn);
        opponentStats.isInvulnerable = false;
    }
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
