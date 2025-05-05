using DG.Tweening;
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
    public TextMeshProUGUI text;
    public GameObject dialogueText;

    [Header("Folder")]
    public Image folder;
    public Button folderButton;
    public Sprite folderChange;

    [Header("Prompts")]
    public GameObject dialoguePrompt;
    public GameObject classSelectPrompt;

    [Header("Scripts")]
    public Class classSelect;
    public PlayableDirector playableDirector;
    public PromptDialogue dialogueSystem;
    public LeanTweenUIManager dialogueTween;

    [HideInInspector] public bool exit;

    private void Start()
    {
        btn.SetActive(false);
        dialogueSystem.GetComponent<PromptDialogue>();
    }

    public void ExitPrompt()
    {
        folder.GetComponent<Image>().sprite = folderChange;
        SoundFX.Play("Click");
        hidePrompt.SetActive(false);

        folderButton.onClick.AddListener(OnButtonClick);
        SoundFX.Play("Ping");
        Play();
    }

    void OnButtonClick()
    {
        dialogueTween.PlayEndAnimation();
        dialogueText.SetActive(false);
        classSelectPrompt.SetActive(true);
        Invoke("Next", 1f);
    }

    void Next()
    {
        dialogueSystem.ShowDialogue();
        classSelectPrompt.LeanMoveLocalY(158, 0.3f).setEaseOutCirc();
        classSelect.dialogueCounter = 2;
        dialogueSystem.SelectDialogue();
    }

    public void Hide()
    {
        SoundFX.Play("Click");

        if (AllSiblingsHidden())
        {
            text.text = "Your data has been leaked.";
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