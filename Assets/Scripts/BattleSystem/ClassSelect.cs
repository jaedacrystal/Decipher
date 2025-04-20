using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;
using Unity.Services.Lobbies.Models;

public class Class : MonoBehaviour
{
    [Header("Cards")]
    public List<Cards> defenseCards;
    public List<Cards> offenseCards;
    public List<Cards> balanceCards;

    [Header("Dialogue")]
    public PromptDialogue dialogue;
    public int dialogueCounter;

    [Header("Prompts")]
    public GameObject selectPrompt;
    public GameObject classPrompt;
    public GameObject game;

    [Header("Button")]
    public Button yesButton;
    public Button noButton;

    [Header("Tween")]
    public LeanTweenUIManager selectTween;
    public LeanTweenUIManager classTween;
    public LevelLoader levelLoader;

    private void Start()
    {
        classPrompt.SetActive(false);
        selectPrompt.gameObject.SetActive(false);
        game.SetActive(false);
    }

    public void Offense()
    {
        
        dialogueCounter = 3;
        dialogue.SelectDialogue();

        classPrompt.LeanMoveLocalX(-154, 0.3f).setEaseOutCirc();
        selectPrompt.SetActive(true);

        yesButton.onClick.AddListener(SaveOffense);
    }

    public void Balanced()
    {
        dialogueCounter = 4;
        dialogue.SelectDialogue();

        classPrompt.LeanMoveLocalX(-154, 0.3f).setEaseOutCirc();
        selectPrompt.SetActive(true);

        yesButton.onClick.AddListener(SaveBalanced);
    }

    public void Defense()
    {
        dialogueCounter = 5;
        dialogue.SelectDialogue();

        classPrompt.LeanMoveLocalX(-154, 0.3f).setEaseOutCirc();
        selectPrompt.SetActive(true);

        yesButton.onClick.AddListener(SaveDefense);
    }

    public void SaveOffense()
    {
        SaveClass(ClassType.Offense, offenseCards);
        NextPrompt();

        selectPrompt.SetActive(false);
        classPrompt.SetActive(false);
    }

    public void SaveBalanced()
    {
        SaveClass(ClassType.Balance, balanceCards);
        NextPrompt();

        selectPrompt.SetActive(false);
        classPrompt.SetActive(false);
    }

    public void SaveDefense()
    {
        SaveClass(ClassType.Defense, defenseCards);
        NextPrompt();

        selectPrompt.SetActive(false);
        classPrompt.SetActive(false);

    }

    public void NoButton()
    {
        classPrompt.LeanMoveLocalX(0, 0.3f).setEaseOutCirc();
        selectTween.PlayEndAnimation();
    }

    void NextPrompt()
    {
        game.SetActive(true);
        Invoke("LoadScene", 3f);
    }

    void LoadScene()
    {
        levelLoader.LoadSelectedScene();
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









