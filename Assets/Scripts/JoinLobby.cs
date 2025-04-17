using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinLobbyButton : MonoBehaviour
{
    public LobbyManager lobby;

    public bool needPassword;
    public string lobbyId;

    public void JoinLobbyButtonPressed()
    {
        LobbyManager.instance.JoinLobby(lobbyId, needPassword);
    }
}
