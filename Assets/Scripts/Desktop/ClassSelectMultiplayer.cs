using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ClassSelectMultiplayer : MonoBehaviour
{
    [Header("Cards")]
    public List<Cards> defenseCards;
    public List<Cards> offenseCards;
    public List<Cards> balanceCards;

    [Header("Dialogue")]
    public PromptDialogue dialogue;
    public int dialogueCounter;

    [Header("Prompts")]
    public GameObject classPrompt;

    [Header("Button")]
    public Sprite[] classIconArray;
    public GameObject classIcon;
    public Button confirm;
    public TextMeshProUGUI classBtnText;

    public Profile multiClass;
    public LeanTweenUIManager tween;

    private void Start()
    {
        classPrompt.SetActive(false);
        classIcon.SetActive(false);
    }

    public void Offense()
    {
        dialogueCounter = 3;
        dialogue.SelectDialogueMultiplayer();
        classBtnText.text = "Confirm";

        confirm.onClick.AddListener(SaveOffense);
    }

    public void Balanced()
    {
        dialogueCounter = 4;
        dialogue.SelectDialogueMultiplayer();
        classBtnText.text = "Confirm";

        confirm.onClick.AddListener(SaveBalanced);
    }

    public void Defense()
    {
        dialogueCounter = 5;
        dialogue.SelectDialogueMultiplayer();
        classBtnText.text = "Confirm";

        confirm.onClick.AddListener(SaveDefense);
    }

    public void SaveOffense()
    {
        SaveClass(ClassType.Offense, offenseCards);

        classIcon.SetActive(true);
        classIcon.GetComponent<Image>().sprite = classIconArray[0];

        Invoke("MoveProfile", 0.5f);
        multiClass.dialogue.dialogue.text = "";
        tween.PlayEndAnimation();
    }

    public void SaveBalanced()
    {
        SaveClass(ClassType.Balance, balanceCards);

        classIcon.SetActive(true);
        classIcon.GetComponent<Image>().sprite = classIconArray[1];

        Invoke("MoveProfile", 0.5f);
        multiClass.dialogue.dialogue.text = "";
        tween.PlayEndAnimation();
    }

    public void SaveDefense()
    {
        SaveClass(ClassType.Defense, defenseCards);

        classIcon.SetActive(true);
        classIcon.GetComponent<Image>().sprite = classIconArray[2];

        Invoke("MoveProfile", 0.5f);
        multiClass.dialogue.dialogue.text = "";
        tween.PlayEndAnimation();
    }

    public void MoveProfile()
    {
        multiClass.profile.LeanMoveLocalY(0, 0.3f).setEaseOutCirc();
        classBtnText.text = "Change Class";
    }

    void SaveClass(ClassType chosenClass, List<Cards> chosenCards)
    {
        PlayerPrefs.SetString("ChosenClass", chosenClass.ToString());

        string cardsJson = JsonUtility.ToJson(new CardListWrapper { cards = chosenCards });
        PlayerPrefs.SetString("ChosenClassCards", cardsJson);

        PlayerPrefs.Save();
    }

    [System.Serializable]
    private class CardListWrapper
    {
        public List<Cards> cards;
    }
}
