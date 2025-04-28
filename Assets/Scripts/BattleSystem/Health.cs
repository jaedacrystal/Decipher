using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
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


    public void TakeDamage(int damage)
    {
        PlayerStats stats = GetComponent<PlayerStats>();

        if (currentHealth > 0)
        {
            CameraShakeManager.instance.CameraShake(impulseSource);
            SoundFX.Play("Hit");

            impactFlash.Flash(spriteRenderer, flashDuration, flashColor, 0.1f, ImpactFlash.FlashType.Damage);

            if (stats != null && stats.isInvulnerable)
            {
                Debug.Log(stats.name + " blocked the damage!");
                return;
            }

            int previousHealth = currentHealth;
            currentHealth -= damage;
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
            retry.gameObject.SetActive(true);
            prompt.gameObject.SetActive(false);
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
        start.LoadNextScene();
    }
}


