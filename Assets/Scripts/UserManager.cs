using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class UserManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject start;
    [SerializeField] private GameObject play;
    [SerializeField] private GameObject connecting;

    public TextMeshProUGUI text;
    public TMP_InputField username;

    private void Start()
    {
        connecting.SetActive(false);
        play.gameObject.SetActive(false);
    }

    public void SignIn()
    {
        SoundFX.Play("Click");

        string playerName = username.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            PlayerPrefs.SetString("PlayerName", playerName);
            PlayerPrefs.Save();
        }

        start.gameObject.SetActive(false);
        play.gameObject.SetActive(true);

        text.SetText("Welcome, " + playerName);
    }
    public void OnClickConnect()
    {
        PhotonNetwork.NickName = username.text;
        GameManager.Instance.IsSingleplayer = false;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Multiplayer"); 
    }
}
