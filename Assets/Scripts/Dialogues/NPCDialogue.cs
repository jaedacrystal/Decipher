using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public int dialogueCounter;
    public TextMeshProUGUI interact;
    public DialogueSystem dialogue;
    private bool isPlayerNear;

    private void Start()
    {
        interact.gameObject.SetActive(false);
        dialogue = FindObjectOfType<DialogueSystem>();
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.F))
        {
            dialogue.showDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        

        if (collision.CompareTag("Player"))
        {
            interact.gameObject.SetActive(true);

            isPlayerNear = true;
            dialogue.canTalk = true;

            dialogue.listCounter = dialogueCounter;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
            dialogue.canTalk = true;

            dialogue.listCounter = dialogueCounter;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interact.gameObject.SetActive(false);

            isPlayerNear = false;
            dialogue.canTalk = false;

            dialogue.listCounter = dialogueCounter;
        }
    }
}
