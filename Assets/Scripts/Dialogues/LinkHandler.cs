using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class LinkHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject email;
    [SerializeField] GameObject prompt;
    [SerializeField] GameObject dialogueBG;
    private TMP_Text textMeshPro;

    private void Start()
    {
        prompt.gameObject.SetActive(false);
    }

    void Awake()
    {
        textMeshPro = GetComponent<TMP_Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, eventData.position, eventData.pressEventCamera);


        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
            if (linkInfo.GetLinkID() == "verify")
            {
                email.gameObject.SetActive(false);
                prompt.gameObject.SetActive(true);
                dialogueBG.gameObject.SetActive(false);
            }
        }
    }
}











