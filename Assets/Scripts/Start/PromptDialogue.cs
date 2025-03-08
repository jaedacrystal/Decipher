using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UIAssistant;

public class PromptDialogue : MonoBehaviour
{
    public int counter;
    public GameObject dialogueBG;
    public TextMeshProUGUI dialogue;
    public TextReveal textReveal;

    public List<Dialogue> dialogues;

    [Serializable]
    public class Dialogue
    {
        [TextArea(3,3)]
        public string dialogue;
    }

    private void Start()
    {
        dialogueBG.SetActive(false);
        dialogue.text = "";
    }

    public void showDialogue()
    {
        dialogueBG.SetActive(true);
        counter = 0;
        textReveal.StartReveal(dialogues[counter].dialogue);
    }

    public void nextDialogue()
    {
        if (counter < dialogues.Count - 1)
        {
            counter++;
            textReveal.StartReveal(dialogues[counter].dialogue);
        }
        else
        {
            counter = 0;
            dialogueBG.SetActive(false);
            dialogue.text = "";
        }
    }
}

