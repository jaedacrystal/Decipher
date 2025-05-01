using System.Runtime.Remoting.Messaging;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Settings : MonoBehaviour
{
    [Header("Settings")]
    public GameObject settingsBG;
    public GameObject check;

    [Header("Menu")]
    public Image menuIcon;
    public Sprite menuIconSelected;
    public Sprite menuIconUnselected;
    public GameObject menuSettings;

    [Header("Audio")]
    public Image audioIcon;
    public Sprite audioIconSelected;
    public Sprite audioIconUnselected;
    public GameObject audioSettings;

    LeanTweenUIManager tween;

    private bool isMuted;

    private void Start()
    {
        tween = GetComponent<LeanTweenUIManager>();
        settingsBG.SetActive(false);
        check.SetActive(false);

        audioSettings.SetActive(false);
        menuSettings.SetActive(false);

        isMuted = PlayerPrefs.GetInt("Muted") == 1;
        AudioListener.pause = isMuted;
    }

    public void ToggleMenu()
    {
        bool isMenuActive = settingsBG.activeSelf;

        if (isMenuActive && tween.playEndAnimation)
        {
            tween.PlayEndAnimation(() => settingsBG.SetActive(false));
        }
        else
        {
            settingsBG.SetActive(true);
            tween.PlayAnimation();
        }

        SoundFX.Play("Click");

        Debug.Log("Called ToggleMenu");
    }

    public void ToggleButton()
    {
        bool isButtonActive = !check.activeSelf;
        check.SetActive(isButtonActive);

        isMuted = !isMuted;
        AudioListener.pause = isMuted;
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);

        SoundFX.Play("Click");
    }

    public void ChooseMenu()
    {
        SoundFX.Play("Click");
        menuIcon.sprite = menuIconSelected;
        menuSettings.SetActive(true);

        audioSettings.SetActive(false);
        audioIcon.sprite = audioIconUnselected;
    }

    public void ChooseAudio()
    {
        SoundFX.Play("Click");
        audioIcon.sprite = audioIconSelected;
        audioSettings.SetActive(true);

        menuSettings.SetActive(false);
        menuIcon.sprite = menuIconUnselected;
    }

    public void Exit()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void Return()
    {
        SceneManager.LoadScene("Menu");
        PhotonNetwork.Disconnect();
        settingsBG.SetActive(false);
    }

    public void Resume()
    {
        tween.PlayEndAnimation(() => settingsBG.SetActive(false));
    }
}



