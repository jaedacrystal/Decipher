using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    [Header("Game Objects")]
    public TextMeshProUGUI initialClassIcon;
    public Button classBtn;
    public GameObject profile;
    public GameObject classPrompt;
    public TextMeshProUGUI player;

    [Header("Scripts")]
    public ClassSelectMultiplayer classSelect;
    public PromptDialogue dialogue;

    void Start()
    {
        dialogue = FindObjectOfType<PromptDialogue>();

        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        player.text = playerName;
    }

    public void ClassSelect()
    {
        classBtn.onClick.AddListener(ShowClassSelect);
    }

    public void ShowClassSelect()
    {
        initialClassIcon.gameObject.SetActive(false);
        classPrompt.SetActive(true);
        profile.LeanMoveLocalY(160, 0.3f).setEaseOutCirc();

        Invoke("Next", 0.3f);
        classBtn.onClick.RemoveAllListeners();
    }

    void Next()
    {
        dialogue.ShowDialogue();
        classSelect.dialogueCounter = 2;
        dialogue.SelectDialogueMultiplayer();
    }
}
