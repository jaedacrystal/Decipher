using UnityEngine;

public class CheckHackPrompt : MonoBehaviour
{
    public GameObject dialogueBG;
    public PromptDialogue dialogue;
    public HackExit hack;
    public GameObject hackPrompt;

    public void afterHackPrompt()
    {
        dialogue.showDialogue();
    }
}
