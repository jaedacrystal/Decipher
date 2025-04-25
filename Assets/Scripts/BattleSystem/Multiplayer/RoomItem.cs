using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI roomPlayers;
    LobbyManager manager;

    private void Start()
    {
        manager = FindObjectOfType<LobbyManager>();
    }

    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }

    public void SetRoomPlayers(int _playersInRoom)
    {
        roomPlayers.text = _playersInRoom.ToString() + "/2";
    }

    public void OnClickRoom()
    {
        manager.JoinRoom(roomName.text);
    }
}
