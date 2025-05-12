using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PhotonHealth : MonoBehaviourPun
{
    [Header("Health Settings")]
    public int maxHealth;
    public int currentHealth;
    public string ownerTag;

    [Header("Text Placeholders")]
    public TextMeshProUGUI prompt;
    public TextMeshProUGUI turnText;

    [Header("Health")]
    public HealthBar healthBar;
    public TextMeshProUGUI healthBarText;

    private string playerName;
    private bool playerCheck;
    public bool isPlayer = false;

    [Header("Flash")]
    public ImpactFlash impactFlash;
    public SpriteRenderer spriteRenderer;
    [HideInInspector] public Color flashColor;
    public float flashDuration;

    public GameObject retry;

    private void Start()
    {
        retry.SetActive(false);
        prompt.gameObject.SetActive(false);
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.gameObject.SetActive(true);
    }

    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return;

        photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    private void RPC_TakeDamage(int damage)
    {
        if (currentHealth > 0)
        {
            CameraShakeManager.instance.CameraShake(GetComponent<Cinemachine.CinemachineImpulseSource>());
            SoundFX.Play("Hit");

            impactFlash.Flash(spriteRenderer, flashDuration, flashColor, 0.1f, ImpactFlash.FlashType.Damage);

            PlayerStats stats = GetComponent<PlayerStats>();
            if (stats != null && stats.isStrongPasswordActive)
            {
                damage = Mathf.CeilToInt(damage * 0.5f);
                Debug.Log($"{stats.name} is protected by Strong Password! Incoming damage reduced by 50%. New damage: {damage}");
            }

            if (stats != null && stats.isProtected)
            {
                Debug.Log(stats.name + " is protected, damage blocked!");
                return;
            }

            float effectiveDamage = damage * (stats != null ? stats.damageTakenMultiplier : 1f);

            int previousHealth = currentHealth;
            currentHealth -= Mathf.CeilToInt(effectiveDamage);
            if (currentHealth < 0) currentHealth = 0;

            DOTween.To(() => previousHealth, x =>
            {
                healthBar.SetHealth(x);
                healthBarText.text = $"{x}/{maxHealth}";
            }, currentHealth, 0.5f).SetEase(Ease.OutQuad);

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    /// <summary>
    /// Heals the player and synchronizes it across all players.
    /// </summary>
    public void Heal(int healAmount)
    {
        if (!photonView.IsMine) return; // Only the owner of this object can apply healing

        photonView.RPC("RPC_Heal", RpcTarget.All, healAmount);
    }

    [PunRPC]
    private void RPC_Heal(int healAmount)
    {
        int previousHealth = currentHealth;
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        impactFlash.Flash(spriteRenderer, flashDuration, flashColor, 0.1f, ImpactFlash.FlashType.Heal);
        SoundFX.Play("Heal");

        DOTween.To(() => previousHealth, x =>
        {
            healthBar.SetHealth(x);
            healthBarText.text = $"{x}/{maxHealth}";
        }, currentHealth, 0.5f).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// Handles the death of the player and synchronizes it across all players.
    /// </summary>
    private void Die()
    {
        photonView.RPC("RPC_Die", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Die()
    {
        playerCheck = isPlayer;
        playerName = playerCheck ? PlayerPrefs.GetString("PlayerName", "Player") : ownerTag;

        prompt.text = playerName + " has been defeated";
        prompt.gameObject.SetActive(true);
        turnText.gameObject.SetActive(false);

        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        GetComponent<SpriteRenderer>().DOFade(0, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            gameObject.SetActive(false);
            healthBar.gameObject.SetActive(false);
        });

        if (isPlayer)
        {
            retry.gameObject.SetActive(true);
            prompt.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Retries the game by reloading the scene.
    /// </summary>
    public void Retry()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.LoadLevel("Battle");
        }
    }
}
