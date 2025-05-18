using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Photon.Pun;

public class MultiplayerHealth : MonoBehaviourPunCallbacks
{
    [Header("Health Settings")]
    public int maxHealth = 100; // Example default value
    public int currentHealth;

    [Header("UI References")]
    public HealthBar healthBar;
    public TextMeshProUGUI healthBarText;

    // Optional: Visual feedback for damage/healing
    public ImpactFlash impactFlash;
    public SpriteRenderer spriteRenderer;
    public float flashDuration = 0.1f;
    public Color damageFlashColor = Color.red;
    public Color healFlashColor = Color.green;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
        if (healthBarText != null)
        {
            healthBarText.text = $"{currentHealth}/{maxHealth}";
        }
    }

    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return; // Only the owner can initiate damage

        int previousHealth = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        photonView.RPC("UpdateHealthUI", RpcTarget.All, currentHealth, previousHealth);

        if (impactFlash != null && spriteRenderer != null)
        {
            impactFlash.Flash(spriteRenderer, flashDuration, damageFlashColor, 0.1f, ImpactFlash.FlashType.Damage);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        if (!photonView.IsMine) return; // Only the owner can initiate healing

        int previousHealth = currentHealth;
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        photonView.RPC("UpdateHealthUI", RpcTarget.All, currentHealth, previousHealth);

        if (impactFlash != null && spriteRenderer != null)
        {
            impactFlash.Flash(spriteRenderer, flashDuration, healFlashColor, 0.1f, ImpactFlash.FlashType.Heal);
        }
    }

    [PunRPC]
    private void UpdateHealthUI(int newHealth, int previousHealth)
    {
        if (healthBar != null)
        {
            DOTween.To(() => previousHealth, x => healthBar.SetHealth(x), newHealth, 0.5f).SetEase(Ease.OutQuad);
        }
        if (healthBarText != null)
        {
            healthBarText.text = $"{newHealth}/{maxHealth}";
        }
    }

    private void Die()
    {
        Debug.Log($"{photonView.Owner.NickName} has been defeated!");
        // Implement your death logic here (e.g., disable controls, show game over UI, etc.)
        // You might want to send an RPC to handle game over on all clients.
    }
}