using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class Health : MonoBehaviourPunCallbacks
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

    public LevelLoader start;
    private GameObject player;

    private CinemachineImpulseSource impulseSource;

    [Header("Flash")]
    public ImpactFlash impactFlash;
    public SpriteRenderer spriteRenderer;
    [HideInInspector] public Color flashColor;
    public float flashDuration;

    public GameObject retry;
    public PhotonCardManager pcm;

    private void Start()
    {
        retry.SetActive(false);
        prompt.gameObject.SetActive(false);
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.gameObject.SetActive(true);

        player = GameObject.FindGameObjectWithTag("Player");
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void PublicTakeDamage(int damage)
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
        }
        else
        {
            TakeDamage(damage); // normal singleplayer call
        }
    }

    [PunRPC]
    public void RPC_TakeDamage(int damage)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        PlayerStats stats = GetComponent<PlayerStats>();

        if (currentHealth > 0)
        {
            CameraShakeManager.instance.CameraShake(impulseSource);
            SoundFX.Play("Hit");

            impactFlash.Flash(spriteRenderer, flashDuration, flashColor, 0.1f, ImpactFlash.FlashType.Damage);

            if (stats != null && stats.isStrongPasswordActive)
            {
                damage = Mathf.CeilToInt(damage * 0.5f);
                Debug.Log($"{stats.name} is protected by Strong Password! Incoming damage reduced by 50%. New damage: {damage}");
            }

            if (stats != null && (stats.isProtected || stats.isInvulnerable))
            {
                Debug.Log(stats.name + " is invulnerable or protected, damage blocked!");
                return;
            }

            float effectiveDamage = damage * stats.damageTakenMultiplier;

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

    public void PublicHeal(int healAmount)
    {
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_Heal", RpcTarget.All, healAmount);
        }
        else
        {
            Heal(healAmount); // normal singleplayer call
        }
    }

    [PunRPC]
    public void RPC_Heal(int healAmount)
    {
        Heal(healAmount);
    }

    public void Heal(int healAmount)
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

    public void Buff()
    {
        impactFlash.Flash(spriteRenderer, flashDuration, flashColor, 0.1f, ImpactFlash.FlashType.Buff);
        SoundFX.Play("Buff");
    }

    public void Debuff()
    {
        impactFlash.Flash(spriteRenderer, flashDuration, flashColor, 0.1f, ImpactFlash.FlashType.Debuff);
        SoundFX.Play("Debuff");
    }

    private void Die()
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
            pcm = FindAnyObjectByType<PhotonCardManager>();

            if (pcm == null)
            {
                retry.gameObject.SetActive(true);
                prompt.gameObject.SetActive(false);
            }

            {
                
            }
            if (pcm.player1)
            {
                retry.gameObject.SetActive(true);
                prompt.gameObject.SetActive(false);
            } else
            {
                retry.gameObject.SetActive(true);
                prompt.gameObject.SetActive(false);
            }
        }
        else
        {
            Invoke("LoadScene", 2f);
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene("Battle");
    }

    void LoadScene()
    {
        start.LoadSelectedScene();
    }
}
