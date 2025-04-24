using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;

public class LobbyManagerMultiplayer : MonoBehaviour
{
    public static LobbyManagerMultiplayer instance {  get; private set; }

    [Header("Lobby Creation")]
    [SerializeField] private GameObject lobbyList;
    [SerializeField] private GameObject createLobbyPrompt;
    [SerializeField] private GameObject optionPrompt;
    [SerializeField] private TMP_InputField lobbyNameField;
    [SerializeField] private TMP_InputField lobbyPasswordField;
    public string joinedLobbyID;

    [Header("Lobby Items")]
    public Transform lobbyItem;
    public TextMeshProUGUI lobbyName;
    public TextMeshProUGUI lobbyPlayerCount;
    [SerializeField] private Transform lobbyParent;

    [Header("Player Profile")]
    public TextMeshProUGUI profileName;
    private Player playerData;

    [Header("Joined lobby")]
    [SerializeField] private GameObject joinedLobby;
    [SerializeField] private Transform playerItemPrefab;
    [SerializeField] private Transform playerListParent;
    [SerializeField] private TextMeshProUGUI joinedLobbyNameText;

    private async void Start()
    {
        instance = this;

        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        profileName.text = playerName;
        
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in: " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        PlayerDataObject playerDataObjectName = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName);
        PlayerDataObject playerDataObjectClass = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "A");

        playerData = new Player(id: AuthenticationService.Instance.PlayerId, data:
        new Dictionary<string, PlayerDataObject> { { "Name", playerDataObjectName }, { "Class", playerDataObjectClass } });
        
    }


    public async void JoinLobby(string lobbyID, bool needPassword)
    {
        if (needPassword)
        {
            try
            {

            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        else
        {
            try
            {
                await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID, new JoinLobbyByIdOptions { Player = playerData });
                joinedLobbyID = lobbyID;

                if (!lobbyList.activeInHierarchy)
                {
                    joinedLobby.gameObject.SetActive(true);
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void ShowLobbies()
    {
        while (Application.isPlaying && lobbyList.activeInHierarchy)
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            foreach (Transform t in lobbyParent)
            {
                Destroy(t.gameObject);
            }

            foreach (Lobby lobby in queryResponse.Results)
            {
                Transform newLobbyItem = Instantiate(lobbyItem, lobbyParent);

                newLobbyItem.GetComponent<JoinLobbyButton>().lobbyId = lobby.Id;
                newLobbyItem.GetComponent<JoinLobbyButton>().needPassword = lobby.HasPassword;

                newLobbyItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = lobby.Name;
                newLobbyItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = lobby.Players.Count + "/" + lobby.MaxPlayers;
            }

            await Task.Delay(5000);
        }
    }
    public void CreateAndShowLobbies()
    {
        CreateLobby();
        ShowLobbyList();
    }

    private async void UpdateLobbyInfo()
    {
        while (Application.isPlaying)
        {
            if (string.IsNullOrEmpty(joinedLobbyID))
            {
                return;
            }

            Lobby lobby = await Lobbies.Instance.GetLobbyAsync(joinedLobbyID);

            joinedLobbyNameText.text = lobby.Name;

            foreach (Transform t in playerListParent)
            {
                Destroy(t.gameObject);
            }

            foreach (Player player in lobby.Players)
            {
                Transform newPlayerItem = Instantiate(playerItemPrefab, playerListParent);
                //newPlayerItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = player.Data["Name"].Value;
                //newPlayerItem.GetChild(2).GetComponent<TextMeshProUGUI>().text = player.Data["Class"].Value;
            }

            await Task.Delay(1000);
        }
    }

    public void ShowLobbyList()
    {
        optionPrompt.gameObject.SetActive(false);
        createLobbyPrompt.SetActive(false);

        if (!lobbyList.activeInHierarchy)
        {
            lobbyList.SetActive(true);
        }

        Invoke("ShowLobbies", 0.5f);
    }

    public async void CreateLobby()
    {

        Lobby createdLobby = null;

        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = false;
        options.Player = playerData;

        try
        {
            string lobbyName = lobbyNameField.text;
            int maxPlayers = 2;
            createdLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyNameField.text, maxPlayers);

            joinedLobbyID = createdLobby.Id;
            UpdateLobbyInfo();

            Debug.Log("Created Lobby: " + lobbyNameField.text);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }

        LobbyHeartbeat(createdLobby);
    }

    private async void LobbyHeartbeat(Lobby lobby)
    {
        while (true)
        {
            if (lobby == null) return;

            await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
            await Task.Delay(15 * 1000);
        }
    }
}
