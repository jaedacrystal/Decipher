using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PromptDialogue : MonoBehaviour
{
    public int counter;
    public GameObject dialogueBG;
    public TextMeshProUGUI dialogue;
    public TextReveal textReveal;

    public bool useSingleSound;
    public string singleTypewriterSound;

    public List<Dialogue> dialogues;

    [Serializable]
    public class Dialogue
    {
        [TextArea(3, 3)] public string dialogue;
        public string typewriterSoundName;
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

        string soundToPlay = GetTypingSound();
        textReveal.SetTypingSound(soundToPlay);
        textReveal.StartReveal(dialogues[counter].dialogue);
    }

    public void nextDialogue()
    {
        if (counter < dialogues.Count - 1)
        {
            counter++;

            string soundToPlay = GetTypingSound();
            textReveal.SetTypingSound(soundToPlay);
            textReveal.StartReveal(dialogues[counter].dialogue);
        }
        else
        {
            counter = 0;
            dialogueBG.SetActive(false);
            dialogue.text = "";
        }
    }

    private string GetTypingSound()
    {
        if (useSingleSound)
        {
            return singleTypewriterSound;
        }

        return dialogues[counter].typewriterSoundName;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (!textReveal.IsFinished)
            {
                textReveal.SkipReveal(dialogues[counter].dialogue);
            }
        }
    }
}
