using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;

public class MultiplayerHealth : MonoBehaviourPunCallbacks
{
    [Header("Health Settings")]
    public int maxHealth;
    [SerializeField] public int currentHealth;
    public string ownerTag;

    [Header("Text Placeholders")]
    public TextMeshProUGUI prompt;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI enemyNameText;

    [Header("Health")]
    public HealthBar healthBar;
    public TextMeshProUGUI healthBarText;

    Player player;
    private string playerName;
    private bool playerCheck;
    public bool isPlayer = false;
    public bool isEnemyPlayer = false;

    public LevelLoader start;

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

        impulseSource = GetComponent<CinemachineImpulseSource>();

        if (photonView.IsMine)
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("PlayerName", "DefaultPlayer");
            playerNameText.text = PhotonNetwork.NickName;

            foreach (KeyValuePair<int, Player> entry in PhotonNetwork.CurrentRoom.Players)
            {
                Player player = entry.Value;

                if (player.IsLocal) continue;

                enemyNameText.text = player.NickName;
                Debug.Log("Opponent Nickname: " + player.NickName);
                break;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return;
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

            photonView.RPC("UpdateHealth", RpcTarget.All, currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void Heal(int healAmount)
    {
        if (!photonView.IsMine) return;

        int previousHealth = currentHealth;
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        impactFlash.Flash(spriteRenderer, flashDuration, flashColor, 0.1f, ImpactFlash.FlashType.Heal);
        SoundFX.Play("Heal");

        photonView.RPC("UpdateHealth", RpcTarget.All, currentHealth);
    }

    private void Die()
    {
        playerCheck = isPlayer || isEnemyPlayer;
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
        else if (isEnemyPlayer)
        {
            Debug.Log("Enemy player defeated!");
            photonView.RPC("HandleEnemyPlayerDefeat", RpcTarget.All);
        }
        else
        {
            Invoke("LoadScene", 2f);
        }
    }

    [PunRPC]
    private void UpdateHealth(int newHealth)
    {
        currentHealth = newHealth;

        healthBar.SetHealth(currentHealth);
        healthBarText.text = $"{currentHealth}/{maxHealth}";
    }

    [PunRPC]
    private void HandleEnemyPlayerDefeat()
    {
        Invoke("LoadScene", 2f);
    }

    public void Retry()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LoadLevel("MultiplayerBattle");
        }
        else
        {
            SceneManager.LoadScene("MultiplayerBattle");
        }
    }

    private void LoadScene()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LoadLevel(start.scene);
        }
        else
        {
            start.LoadNextScene();
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!newPlayer.IsLocal)
        {
            enemyNameText.text = newPlayer.NickName;
            Debug.Log("Opponent Joined: " + newPlayer.NickName);
        }
    }
}