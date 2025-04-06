using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    public void ShowDialogue()
    {
        dialogueBG.SetActive(true);
        dialogue.gameObject.SetActive(true);
        counter = 0;

        string soundToPlay = GetTypingSound();
        textReveal.SetTypingSound(soundToPlay);
        textReveal.StartReveal(dialogues[counter].dialogue);
    }


    public void NextDialogue()
    {
        dialogueBG.SetActive(true);
        dialogue.gameObject.SetActive(true);
        counter = 1;

        string soundToPlay = GetTypingSound();
        textReveal.SetTypingSound(soundToPlay);
        textReveal.StartReveal(dialogues[counter].dialogue);
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
