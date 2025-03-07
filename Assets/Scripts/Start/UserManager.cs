using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserManager : MonoBehaviour
{
    [SerializeField] GameObject start;
    [SerializeField] GameObject play;

    public TextMeshProUGUI text;
    public TextMeshProUGUI username;

    private void Start()
    {
        play.gameObject.SetActive(false);
    }

    public void SignIn()
    {
        start.gameObject.SetActive(false);
        play.gameObject.SetActive(true);

        text.SetText("Welcome, " + username.text);
    }
}
