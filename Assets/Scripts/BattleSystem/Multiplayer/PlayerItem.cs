using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviourPunCallbacks
{
    LobbyManager lobby;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerClass;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    Player player;

    private void Start()
    {
        lobby = FindObjectOfType<LobbyManager>();
    }

    public void SetPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;

        SelectedClass();
        UpdatePlayerClass(player);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (player == targetPlayer)
        {
            UpdatePlayerClass(targetPlayer);
        }
    }

    void UpdatePlayerClass(Player player)
    {
        if (player.CustomProperties.ContainsKey("playerClass"))
        {
            playerClass.text = player.CustomProperties["playerClass"].ToString();
            playerProperties["playerClass"] = player.CustomProperties["playerClass"].ToString();
        } else
        {
            playerClass.text = "None";
            playerProperties["playerClass"] = "None";
        }
    }

    public void SelectedClass()
    {
        string savedClass = PlayerPrefs.GetString("ChosenClass", "None");
        playerProperties["playerClass"] = savedClass;

        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }
}
