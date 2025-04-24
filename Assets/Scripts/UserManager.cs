using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class UserManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject start;
    [SerializeField] GameObject play;

    public TextMeshProUGUI text;
    public TMP_InputField username;

    private void Start()
    {
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
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Multiplayer"); 
    }
}
