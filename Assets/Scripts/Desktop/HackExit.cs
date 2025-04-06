using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HackExit : MonoBehaviour
{
    public GameObject hidePrompt;
    public GameObject btn;
    public Image folder;
    public Button folderButton;
    public Sprite folderChange;
    public TextMeshProUGUI text;

    public PlayableDirector playableDirector;

    [HideInInspector] public bool exit;

    private void Start()
    {
        btn.SetActive(false);
    }

    public void ExitPrompt()
    {
        folder.GetComponent<Image>().sprite = folderChange;
        SoundFX.Play("Click");
        hidePrompt.SetActive(false);

        folderButton.onClick.AddListener(OnButtonClick);
        Play();
    }

    void OnButtonClick()
    {
        SceneManager.LoadScene("BattleTutorial");
    }

    public void Hide()
    {
        SoundFX.Play("Click");

        if (AllSiblingsHidden())
        {
            text.text = "Something's wrong...";
            btn.SetActive(true);
        }
    }

    private bool AllSiblingsHidden()
    {
        Transform parent = hidePrompt.transform.parent;

        foreach (Transform sibling in parent)
        {
            if (sibling.gameObject == hidePrompt) continue;


            if (sibling.gameObject.activeSelf) return false;
        }

        return true;
    }

    private void Play()
    {
        playableDirector.Play();
    }
}