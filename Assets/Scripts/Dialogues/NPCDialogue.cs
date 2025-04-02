using TMPro;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public int dialogueCounter;
    public TextMeshProUGUI interact;
    public GameObject cardClass;
    public DialogueSystem dialogue;

    private bool isPlayerNear;

    [SerializeField] public LevelLoader start;

    private void Start()
    {
        cardClass.gameObject.SetActive(false);
        interact.gameObject.SetActive(false);
        dialogue = FindObjectOfType<DialogueSystem>();
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKey(KeyCode.F))
        {
            interact.gameObject.SetActive(false);
            dialogue.listCounter = dialogueCounter;
            dialogue.ShowDialogue();
        }
    }

    public void CheckDialogueEnd()
    {
        if (dialogue.listCounter == dialogueCounter && dialogue.counter == 0)
        {
            cardClass.gameObject.SetActive(true);
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