using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Asteroids;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField lobbyNameField;
    public TextMeshProUGUI lobbyName;

    [SerializeField] private GameObject createLobbyPrompt;
    [SerializeField] private GameObject roomPanel;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItems = new List<RoomItem>();
    public Transform contentObject;

    private void Start()
    {
        PhotonNetwork.JoinLobby();
    }

    public void CreateLobby()
    {
        PhotonNetwork.CreateRoom(lobbyNameField.text, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        createLobbyPrompt.SetActive(false);
        roomPanel.SetActive(true);
        lobbyName.text = "Lobby: " + PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomList(roomList);
    }
    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomItems)
        {
            Destroy(item.gameObject);
        }
        roomItems.Clear();

        foreach (RoomInfo room in list)
        {
            //if (room.RemovedFromList) continue;
            RoomItem newRoomItem = Instantiate(roomItemPrefab, contentObject);
            newRoomItem.SetRoomName(room.Name);
            roomItems.Add(newRoomItem);
        }
    }
}
