using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerClass;

    public void SetPlayerInfo(Player _player)
    {
        string savedClass = PlayerPrefs.GetString("ChosenClass", "None");
        if (savedClass == "None") return;

        playerName.text = _player.NickName;
        playerClass.text = savedClass;
    }
}
