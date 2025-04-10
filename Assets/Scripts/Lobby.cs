using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor.Overlays;
using UnityEngine.UI;

public class TestLobby : MonoBehaviour
{
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

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in: " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
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

                newLobbyItem.GetChild(0).GetComponent<TextMeshProUGUI>().text = lobby.Name;
                newLobbyItem.GetChild(1).GetComponent<TextMeshProUGUI>().text = lobby.Players.Count + "/" + lobby.MaxPlayers;
            }

            await Task.Delay(5000);
        }
    }

    public void ShowAndCreateLobbyList()
    {
        CreateLobby();
        ShowLobbyList();
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
        try
        {
            string lobbyName = lobbyNameField.text;
            int maxPlayers = 2;
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyNameField.text, maxPlayers);

            joinedLobbyID = lobby.Id;

            Debug.Log("Created Lobby: " + lobbyNameField.text);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }
}
