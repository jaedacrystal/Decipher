using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Cards : ScriptableObject
{
    public Sprite artwork;
    public string cardName;

    [TextArea(3, 3)]
    public string desc;

    public int bandwidth;
    public int effectValue;

    public ClassType classType;
    public EffectType effectType;
    public TargetType target;

    public void ApplyEffect(GameObject player, GameObject opponent)
    {
        Health playerHealth = player.GetComponent<Health>();
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        Health opponentHealth = opponent.GetComponent<Health>();
        PlayerStats opponentStats = opponent.GetComponent<PlayerStats>();

        GameObject targetObject = target == TargetType.Player ? player : opponent;
        Health targetHealth = targetObject.GetComponent<Health>();
        PlayerStats targetStats = targetObject.GetComponent<PlayerStats>();

        switch (effectType)
        {
            case EffectType.Attack:
                targetHealth.TakeDamage(effectValue);
                Debug.Log(targetObject.name + " took " + effectValue + " damage!");
                break;

            case EffectType.Defense:
                targetStats.IncreaseDefense(effectValue);
                break;

            case EffectType.Heal:
                targetHealth.Heal(effectValue);
                break;

            case EffectType.Draw:
                CardManager cardManager = FindObjectOfType<CardManager>();
                cardManager.DrawMultipleCards(effectValue);
                break;

            case EffectType.ShieldAndRetaliate:
                TurnManager turnManager = TurnManager.Instance;
                turnManager.StartCoroutine(ApplyShieldAndRetaliate(playerHealth, opponentStats, effectValue));
                break;

            default:
                break;
        }
    }

    private IEnumerator ApplyShieldAndRetaliate(Health playerHealth, PlayerStats opponentStats, int effectValue)
    {
        TurnManager turnManager = TurnManager.Instance;

        playerHealth.TakeDamage(effectValue);
        Debug.Log(playerHealth.name + " took " + effectValue + " immediate damage!");

        yield return new WaitUntil(() => !turnManager.isPlayerTurn);

        opponentStats.isInvulnerable = true;
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
    ShieldAndRetaliate
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