using UnityEngine;
using TMPro;
using Photon.Pun;

public class EnemyManager : MonoBehaviourPunCallbacks
{
    [Header("Enemy Settings")]
    public GameObject enemyPlayer;
    public TextMeshProUGUI enemyNameText;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            InitializeEnemy();
        }
    }

    private void InitializeEnemy()
    {
        Debug.Log($"Initializing enemy: {gameObject.name}");

        MultiplayerHealth health = GetComponent<MultiplayerHealth>();
        if (health != null)
        {
            health.isPlayer = false;
            health.isEnemyPlayer = true;
            health.enemyNameText.text = photonView.Owner.NickName;
        }
    }
}
