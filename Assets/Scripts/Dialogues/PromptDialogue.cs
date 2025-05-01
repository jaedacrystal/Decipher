using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PromptDialogue : MonoBehaviour
{
    public int counter;
    public GameObject dialogueBG;
    public TextMeshProUGUI dialogue;
    public TextReveal textReveal;

    public bool useSingleSound;
    public string singleTypewriterSound;

    public List<Dialogue> dialogues;

    public Class classSelect;
    public ClassSelectMultiplayer classSelectMultiplayer;
    public LeanTweenUIManager tween;

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

        if (classSelectMultiplayer.gameObject.activeInHierarchy)
        {
            if (classSelectMultiplayer == null)
            {
                return;
            }
            else
            {
                classSelectMultiplayer = FindObjectOfType<ClassSelectMultiplayer>();
            }
        }
        else
        {
            if (classSelect == null)
            {
                return;
            }
            else
            {
                classSelect = FindObjectOfType<Class>();
            }
        }

        if (tween == null)
        {
            return;
        }
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

    public void NormalDialogue()
    {
        dialogueBG.SetActive(true);
        dialogue.gameObject.SetActive(true);
        counter = 0;

        string soundToPlay = GetTypingSound();
        textReveal.SetTypingSound(soundToPlay);
        textReveal.StartReveal(dialogues[counter].dialogue);
    }

    public void HideDialogue()
    {
        dialogue.text = "";
        tween.PlayEndAnimation();
    }

    public void SecondDialogue()
    {
        dialogueBG.SetActive(true);
        dialogue.gameObject.SetActive(true);
        counter = 1;

        string soundToPlay = GetTypingSound();
        textReveal.SetTypingSound(soundToPlay);
        textReveal.StartReveal(dialogues[counter].dialogue);
    }

    public void SelectDialogue()
    {
        if (classSelect != null)
        {
            dialogueBG.SetActive(true);
            dialogue.gameObject.SetActive(true);
            counter = classSelect.dialogueCounter;

            string soundToPlay = GetTypingSound();
            textReveal.SetTypingSound(soundToPlay);
            textReveal.StartReveal(dialogues[counter].dialogue);
        }
    }

    public void SelectDialogueMultiplayer()
    {
        if (classSelectMultiplayer != null)
        {
            dialogueBG.SetActive(true);
            dialogue.gameObject.SetActive(true);
            counter = classSelectMultiplayer.dialogueCounter;

            string soundToPlay = GetTypingSound();
            textReveal.SetTypingSound(soundToPlay);
            textReveal.StartReveal(dialogues[counter].dialogue);
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
