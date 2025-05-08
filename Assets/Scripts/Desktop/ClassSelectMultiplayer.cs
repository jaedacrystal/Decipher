using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ClassSelectMultiplayer : MonoBehaviour
{
    [Header("Cards")]
    public List<Cards> defenseCards;
    public List<Cards> offenseCards;
    public List<Cards> balanceCards;

    [Header("Dialogue")]
    public PromptDialogue dialogue;
    public int dialogueCounter;

    [Header("Button")]
    public Sprite[] classIconArray;
    public GameObject classIcon;
    public Button confirm;
    public TextMeshProUGUI classBtnText;

    public GameObject classSelection;
    public Profile profile;
    public LeanTweenUIManager tween;

    private void Start()
    {
        classSelection.SetActive(false);
        classIcon.SetActive(false);
    }

    public void Offense()
    {
        if (classSelection.activeSelf)
        {
            dialogueCounter = 3;
            dialogue.SelectDialogueMultiplayer();
        }

        classBtnText.text = "Confirm";
        profile.btHover.buttonHover = true;
        confirm.enabled = true;
        confirm.onClick.AddListener(SaveOffense);
    }

    public void Balanced()
    {
        if (classSelection.activeSelf)
        {
            dialogueCounter = 4;
            dialogue.SelectDialogueMultiplayer();
        }

        classBtnText.text = "Confirm";
        profile.btHover.buttonHover = true;
        confirm.enabled = true;
        confirm.onClick.AddListener(SaveBalanced);
    }

    public void Defense()
    {
        if (classSelection.activeSelf)
        {
            dialogueCounter = 5;
            dialogue.SelectDialogueMultiplayer();
        }

        classBtnText.text = "Confirm";
        profile.btHover.buttonHover = true;
        confirm.enabled = true;
        confirm.onClick.AddListener(SaveDefense);
    }

    public void SaveOffense()
    {
        SaveClass(ClassType.Offense, offenseCards);

        classIcon.SetActive(true);
        classIcon.GetComponent<Image>().sprite = classIconArray[0];
        confirm.onClick.RemoveAllListeners();

        Invoke("MoveProfile", 0.5f);

        dialogue.HideDialogue();

        classSelection.SetActive(false);
    }

    public void SaveBalanced()
    {
        SaveClass(ClassType.Balance, balanceCards);

        classIcon.SetActive(true);
        classIcon.GetComponent<Image>().sprite = classIconArray[1];
        confirm.onClick.RemoveAllListeners();

        Invoke("MoveProfile", 0.5f);
        dialogue.HideDialogue();

        classSelection.SetActive(false);
    }

    public void SaveDefense()
    {
        SaveClass(ClassType.Defense, defenseCards);

        classIcon.SetActive(true);
        classIcon.GetComponent<Image>().sprite = classIconArray[2];
        confirm.onClick.RemoveAllListeners();

        Invoke("MoveProfile", 0.5f);

        dialogue.HideDialogue();

        classSelection.SetActive(false);
    }

    public void MoveProfile()
    {
        profile.profile.LeanMoveLocalY(0, 0.3f).setEaseOutCirc();
        classBtnText.text = "Change Class";
        profile.classBtn.onClick.AddListener(ChangeClass);

        profile.btHover.buttonHover = true;
        confirm.enabled = true;
    }

    public void ChangeClass()
    {
        classBtnText.text = "Choose Class";
        classIcon.SetActive(false);

        profile.ShowClassSelect();
    }

    void SaveClass(ClassType chosenClass, List<Cards> chosenCards)
    {
        PlayerPrefs.SetString("ChosenClass", chosenClass.ToString());

        string cardsJson = JsonUtility.ToJson(new CardListWrapper { cards = chosenCards });
        PlayerPrefs.SetString("ChosenClassCards", cardsJson);

        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
        playerProperties["playerClass"] = chosenClass.ToString();
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);

        PlayerPrefs.Save();
    }

    [System.Serializable]
    private class CardListWrapper
    {
        public List<Cards> cards;
    }
}
