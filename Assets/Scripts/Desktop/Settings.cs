using UnityEngine;


public class Settings : MonoBehaviour
{
    public GameObject settingsBG;
    public GameObject check;

    LeanTweenUIManager tween;

    private bool isMuted;

    private void Start()
    {
        tween = GetComponent<LeanTweenUIManager>();
        settingsBG.SetActive(false);
        check.SetActive(false);

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
}



