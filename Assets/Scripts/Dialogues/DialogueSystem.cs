using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public int counter;
    public int listCounter;
    [HideInInspector] public bool canTalk;
    private bool playerCheck;

    public GameObject dialogueBG;
    public TextMeshProUGUI player;
    public TextMeshProUGUI dialogue;
    public TextReveal textReveal;

    public string playerTypingSound;

    [Serializable]
    public class DialogueList
    {
        public string title;
        public bool useSingleSoundForList;
        public string singleTypewriterSound;
        public List<Dialogue> dialogues;
    }

    public List<DialogueList> dialogueList;

    [Serializable]
    public class Dialogue
    {
        public bool isPlayer = false;
        public string playerName;
        [TextArea(3, 3)]
        public string dialogue;
        public string typewriterSoundName;
    }

    private void Start()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");

        playerCheck = dialogueList[listCounter].dialogues[counter].isPlayer;

        if (playerCheck)
        {
            player.text = playerName;
        }
        else
        {
            player.text = dialogueList[listCounter].dialogues[counter].playerName;
        }

        Invoke("ShowDialogue", 1);
        dialogue.text = "";
    }

    private void Update()
    {
        if (canTalk && Input.GetKeyDown(KeyCode.F))
        {
            dialogueBG.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (!textReveal.IsFinished)
            {
                textReveal.SkipReveal(dialogueList[listCounter].dialogues[counter].dialogue);
            }
            else
            {
                NextDialogue();
            }
        }
    }

    public void HideDialogue()
    {
        dialogueBG.SetActive(false);
    }

    public void ShowDialogue()
    {
        dialogueBG.SetActive(true);
        counter = 0;

        string soundToPlay = GetTypingSound();
        textReveal.SetTypingSound(soundToPlay);
        textReveal.StartReveal(dialogueList[listCounter].dialogues[counter].dialogue);
    }

    public void NextDialogue()
    {
        if (counter < dialogueList[listCounter].dialogues.Count - 1)
        {
            counter++;

            if (listCounter == 0 && counter == 1)
            {
                Invoke("HideDialogue", 5);
            }

            playerCheck = dialogueList[listCounter].dialogues[counter].isPlayer;

            if (playerCheck)
            {
                player.text = PlayerPrefs.GetString("PlayerName", "Player");
            }
            else
            {
                player.text = dialogueList[listCounter].dialogues[counter].playerName;
            }

            string soundToPlay = GetTypingSound();
            textReveal.SetTypingSound(soundToPlay);
            textReveal.StartReveal(dialogueList[listCounter].dialogues[counter].dialogue);
        }
        else
        {
            counter = 0;
            dialogueBG.SetActive(false);
            dialogue.text = "";

            FindObjectOfType<NPCDialogue>().CheckDialogueEnd();
        }
    }

    private string GetTypingSound()
    {
        var currentDialogue = dialogueList[listCounter].dialogues[counter];

        if (currentDialogue.isPlayer)
        {
            return playerTypingSound;
        }

        if (dialogueList[listCounter].useSingleSoundForList)
        {
            return dialogueList[listCounter].singleTypewriterSound;
        }

        return currentDialogue.typewriterSoundName;
    }
}
