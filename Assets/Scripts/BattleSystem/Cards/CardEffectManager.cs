using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    public static CardEffectManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayCard(Cards card, GameObject player, GameObject opponent)
    {
        if (card == null) return;

        PlayerStats playerStats = player.GetComponent<PlayerStats>();

        if (playerStats != null && playerStats.CanPlayCard(card.bandwidth))
        {
            playerStats.UseBandwidth(card.bandwidth);
            card.ApplyEffect(player, opponent);
        }
    }
}

