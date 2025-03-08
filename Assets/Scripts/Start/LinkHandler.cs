using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Collections;
using UIAssistant;

public class LinkHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public LevelLoader start;
    [SerializeField] GameObject email;
    [SerializeField] GameObject prompt;
    [SerializeField] GameObject notif;
    [SerializeField] GameObject dialogueBG;
    [SerializeField] public Button myButton;

    public TextRevealer textReveal;
    private TMP_Text textMeshPro;

    public PromptDialogue dialogue;

    private void Start()
    {
        prompt.gameObject.SetActive(false);
        notif.gameObject.SetActive(false);
    }

    void Awake()
    {
        textMeshPro = GetComponent<TMP_Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, Input.mousePosition, null);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
            if (linkInfo.GetLinkID() == "verify")
            {
                email.gameObject.SetActive(false);
                prompt.gameObject.SetActive(true);
                notif.gameObject.SetActive(true);
                dialogueBG.gameObject.SetActive(false);

                Invoke("NextDialogue", 4);

                myButton.onClick.AddListener(OnButtonClick);
            }
        }
    }

    public void NextDialogue()
    {
        dialogue.showDialogue();
        dialogue.nextDialogue();
    }

    void OnButtonClick()
    {
        start.LoadNextScene();
    }
}
