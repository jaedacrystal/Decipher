using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification : MonoBehaviour
{
    public PromptDialogue dialogue;

    public void ShowDialogue()
    {
        dialogue.showDialogue();
    }
}
