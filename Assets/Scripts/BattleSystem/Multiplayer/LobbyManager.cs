using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Asteroids;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Lobby")]
    [SerializeField] private TMP_InputField lobbyNameField;
    public TextMeshProUGUI lobbyName;
    [SerializeField] private GameObject createLobbyPrompt;
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private GameObject lobbyPanel;

    [Header("Rooms")]
    public RoomItem roomItemPrefab;
    List<RoomItem> roomItems = new List<RoomItem>();
    public Transform contentObject;

    public float timeUpdates = 0.5f;
    float nextUpdateTime;

    public GameObject playButton;

    [Header("Player")]
    private List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerPrefab;
    public Transform playerParent;

    public LeanTweenUIManager tween;

    private void Start()
    {
        PhotonNetwork.JoinLobby();
        tween = FindObjectOfType<LeanTweenUIManager>();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }
    }

    public void CreateLobby()
    {
        PhotonNetwork.CreateRoom(lobbyNameField.text, new RoomOptions { MaxPlayers = 2, BroadcastPropsChangeToAll = true });
    }

    public override void OnJoinedRoom()
    {
        createLobbyPrompt.SetActive(false);
        roomPanel.SetActive(true);
        lobbyName.text = "Lobby: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeUpdates;
        }
        
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
            RoomItem newRoomItem = Instantiate(roomItemPrefab, contentObject);
            newRoomItem.SetRoomName(room.Name);
            newRoomItem.SetRoomPlayers(room.PlayerCount);
            roomItems.Add(newRoomItem);
        }
    }

    public void JoinRoom(string roomName)
    {
        lobbyPanel.SetActive(false);
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnClickPlayButton()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel("MultiplayerBattle");
        }
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        tween.PlayEndAnimation();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public void UpdatePlayerList()
    {
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerPrefab, playerParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            playerItemsList.Add(newPlayerItem);
        }
    }
}
