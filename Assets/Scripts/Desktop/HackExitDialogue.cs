using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackExitDialogue : MonoBehaviour
{
    PromptDialogue dialogue;

    public void startDialogue()
    {
        dialogue.showDialogue();
        dialogue.nextDialogue();
    }
}
