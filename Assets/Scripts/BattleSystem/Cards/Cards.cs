using System.Collections;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Scripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Cards : ScriptableObject
{
    public Sprite artwork;
    public string cardName;

    [TextArea(3, 3)]
    public string desc;
    [TextArea(3, 3)]
    public string flavorTxt;

    public int bandwidth;
    public int effectValue;
    public int effectRounds;

    [Header("Class Effects")]
    public ClassType classType;
    public EffectType effectType;
    public TargetType target;

    public bool isSingleplayer = true;
    public TextMeshProUGUI cardText;

    [Preserve]
    public void ApplyEffect ( PlayerStats playerStats, Health playerHealth, PlayerStats opponentStats, Health opponentHealth, TargetType target ) {
        
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

    private void ApplyEffectLogic(Health targetHealth, PlayerStats targetStats)
    {
        switch (effectType)
        {
            case EffectType.Attack:
                targetHealth.TakeDamage(effectValue);
                Debug.Log(targetHealth.name + " took " + effectValue + " damage!");
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
                {
                    cardManager.DrawMultipleCards(effectValue);
                }
                else
                {
                    Debug.LogError("CardManager not found!");
                }
                break;

            case EffectType.ShieldAndRetaliate:
                TurnManager turnManager = TurnManager.Instance;
                if (turnManager != null)
                {
                    turnManager.StartCoroutine(ApplyShieldAndRetaliate(targetHealth, targetStats, effectValue));
                }
                else
                {
                    Debug.LogError("TurnManager is not initialized!");
                }
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

            default:
                break;
        }
    }

    private void ApplyForgetPasswordFromGraveyard()
    {
        CardManager cardManager = FindObjectOfType<CardManager>();
        Discard discard = FindObjectOfType<Discard>();
        if (cardManager == null || discard == null)
        {
            Debug.LogError("CardManager or Discard not found!");
            return;
        }

        Transform graveyardTransform = discard.graveyard.transform;

        int graveyardCount = graveyardTransform.childCount;
        if (graveyardCount == 0)
        {
            Debug.LogWarning("No cards in the graveyard.");
            return;
        }

        Health playerHealth = cardManager.playerHealth;
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(5);
        }

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

        Debug.Log("Forget Password retrieved: " + cardTransform.name);
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

        Health playerHealth = targetStats.GetComponent<Health>();
        Debug.Log("Strong Password applied! Target takes half damage and ignores debuffs for a turn.");

        cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
        cardText.text = "Strong Password applied! Target takes half damage and ignores debuffs for a turn.";
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
        Debug.Log("Strong Password expired. Target is now vulnerable again.");
    }

    private void StartBurnEffect(dynamic targetHealth, int effectValue)
    {
        PlayerStats targetStats = targetHealth.GetComponent<PlayerStats>();
        if (targetStats != null)
        {
            targetStats.StartBurnEffect(effectRounds, effectValue);
        }
        else
        {
            Debug.LogError("Target does not have PlayerStats to apply burn effect.");
        }
    }

    private void ApplyDataLeak(PlayerStats targetStats)
    {
        targetStats.damageTakenMultiplier *= 1.2f;

        cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
        cardText.text = "Opponent is now vulnerable to 20% more damage!";
        cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1);
        cardText.gameObject.SetActive(true);
        cardText.DOFade(1, 1f).OnComplete(() => cardText.DOFade(0, 1f));

        Debug.Log("Target is now vulnerable to 20% more damage.");
    }

    private void ApplyZeroDayExploit(Health targetHealth, PlayerStats targetStats, PlayerStats opponentStats)
    {
        int damage = effectValue;

        if (!targetStats.isProtected)
        {
            damage *= 2;
        }

        targetHealth.TakeDamage(damage);
        Debug.Log(targetHealth.name + " took " + damage + " damage due to Zero-Day Exploit!");

        cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
        cardText.text = "Opponent took " + damage + " damage!";
        cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1);
        cardText.gameObject.SetActive(true);
        cardText.DOFade(1, 1f).OnComplete(() => cardText.DOFade(0, 1f));

        if (targetStats.isProtected)
        {
            Debug.Log(targetHealth.name + " was protected from the exploit.");
        }
    }

    private IEnumerator ApplyShieldAndRetaliate(Health playerHealth, PlayerStats opponentStats, int effectValue)
    {
        TurnManager turnManager = TurnManager.Instance;

        playerHealth.TakeDamage(effectValue);
        Debug.Log(playerHealth.name + " took " + effectValue + " immediate damage!");

        yield return new WaitUntil(() => !turnManager.isPlayerTurn);

        opponentStats.isInvulnerable = true;

        cardText = GameObject.Find("CardText").GetComponent<TextMeshProUGUI>();
        cardText.text = "You are now invulnerable!";
        cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1);
        cardText.gameObject.SetActive(true);
        cardText.DOFade(1, 1f).OnComplete(() => cardText.DOFade(0, 1f));

        Debug.Log(opponentStats.name + " is now invulnerable!");

        yield return new WaitUntil(() => turnManager.isPlayerTurn);

        opponentStats.isInvulnerable = false;
        Debug.Log(opponentStats.name + " is no longer invulnerable.");
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