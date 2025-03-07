using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class ClickableTextMeshPro : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public LevelLoader start;
    [SerializeField] GameObject email;
    [SerializeField] GameObject prompt;
    [SerializeField] GameObject notif;
    [SerializeField] public Button myButton;
    private TMP_Text textMeshPro;

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
                myButton.onClick.AddListener(OnButtonClick);
            }
        }
    }

    void OnButtonClick()
    {
        start.LoadNextScene();
    }
}
