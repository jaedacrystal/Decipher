using System.Collections;
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

    public ClassType classType;
    public EffectType effectType;
    public TargetType target;

    //public void ApplyEffect(GameObject player, GameObject opponent)
    //{
    //    MultiplayerHealth playerHealth = player.GetComponent<MultiplayerHealth>();
    //    PlayerStats playerStats = player.GetComponent<PlayerStats>();
    //    MultiplayerHealth opponentHealth = opponent.GetComponent<MultiplayerHealth>();
    //    PlayerStats opponentStats = opponent.GetComponent<PlayerStats>();

    //    GameObject targetObject = target == TargetType.Player ? player : opponent;
    //    MultiplayerHealth targetHealth = targetObject.GetComponent<MultiplayerHealth>();
    //    PlayerStats targetStats = targetObject.GetComponent<PlayerStats>();

    //    switch (effectType)
    //    {
    //        case EffectType.Attack:
    //            targetHealth.TakeDamage(effectValue);
    //            Debug.Log(targetObject.name + " took " + effectValue + " damage!");
    //            break;

    //        case EffectType.Defense:
    //            targetStats.IncreaseDefense(effectValue);
    //            break;

    //        case EffectType.DefenseAndDebuff:
    //            targetStats.IncreaseDefenseAndDebuff(effectValue);
    //            break;

    //        case EffectType.Heal:
    //            targetHealth.Heal(effectValue);
    //            break;

    //        case EffectType.Draw:
    //            CardManager cardManager = FindObjectOfType<CardManager>();
    //            cardManager.DrawMultipleCards(effectValue);
    //            break;

    //        case EffectType.ShieldAndRetaliate:
    //            TurnManager turnManager = TurnManager.Instance;
    //            turnManager.StartCoroutine(ApplyShieldAndRetaliate(playerHealth, opponentStats, effectValue));
    //            break;

    //        default:
    //            break;
    //    }
    //}

    public void ApplyEffect(GameObject player, GameObject opponent)
    {
        if (player == null || opponent == null)
        {
            Debug.LogError("Player or opponent GameObject is null!");
            return;
        }

        MultiplayerHealth playerHealth = player.GetComponent<MultiplayerHealth>();
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        MultiplayerHealth opponentHealth = opponent.GetComponent<MultiplayerHealth>();
        PlayerStats opponentStats = opponent.GetComponent<PlayerStats>();

        if (playerHealth == null || playerStats == null || opponentHealth == null || opponentStats == null)
        {
            Debug.LogError("One or more required components are missing from player or opponent!");
            return;
        }

        GameObject targetObject = target == TargetType.Player ? player : opponent;
        MultiplayerHealth targetHealth = targetObject.GetComponent<MultiplayerHealth>();
        PlayerStats targetStats = targetObject.GetComponent<PlayerStats>();

        if (targetHealth == null || targetStats == null)
        {
            Debug.LogError("Target (player or opponent) is missing required components!");
            return;
        }

        switch (effectType)
        {
            case EffectType.Attack:
                targetHealth.TakeDamage(effectValue);
                Debug.Log(targetObject.name + " took " + effectValue + " damage!");
                break;

            case EffectType.Defense:
                targetStats.IncreaseDefense(effectValue);
                break;

            case EffectType.DefenseAndDebuff:
                targetStats.IncreaseDefenseAndDebuff(effectValue);
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
                    turnManager.StartCoroutine(ApplyShieldAndRetaliate(playerHealth, opponentStats, effectValue));
                }
                else
                {
                    Debug.LogError("TurnManager is not initialized!");
                }
                break;

            default:
                break;
        }
    }


    private IEnumerator ApplyShieldAndRetaliate(MultiplayerHealth playerHealth, PlayerStats opponentStats, int effectValue)
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
    DefenseAndDebuff,
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