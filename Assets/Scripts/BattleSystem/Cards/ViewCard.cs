using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ViewCard : MonoBehaviour
{
    [Header("Scale")]
    public float initialScale;
    public float scale;
    public float transitionSpeed;

    [Header("Position")]
    private int originalSiblingIndex;
    private RectTransform transformObject;
    [HideInInspector] public Vector3 originalLocalPosition;

    [Header("Description Prompt")]
    private CardDisplay cardDisplay;
    public TextMeshProUGUI desc;
    public GameObject cardDesc;

    public CardDisplay cardData;

    public bool isClicked;

    private void Start()
    {
        transformObject = GetComponent<RectTransform>();
        originalSiblingIndex = transform.GetSiblingIndex();
        cardDisplay = FindObjectOfType<CardDisplay>();
        cardData = GetComponent<CardDisplay>();

        cardDesc = GameObject.Find("DescriptionPrompt");
        desc = cardDesc.GetComponentInChildren<TextMeshProUGUI>();

        cardDisplay.descPrompt.gameObject.SetActive(false);
    }

    public void cardClicked()
    {
        if (isClicked)
        {
            transform.DOLocalMove(originalLocalPosition, transitionSpeed);
            transform.DOScale(initialScale, transitionSpeed);

            transform.SetSiblingIndex(originalSiblingIndex);
            isClicked = false;

            cardDisplay.tween.PlayEndAnimation();
        }
        else
        {
            originalLocalPosition = transformObject.localPosition;

            Vector3 targetPosition = new Vector3(0, 600f, 0);
            transform.DOScale(scale, transitionSpeed);
            transform.DOLocalMove(targetPosition, transitionSpeed);

            transform.SetAsLastSibling();
            isClicked = true;

            cardDisplay.descPrompt.gameObject.SetActive(true);
            desc.text = cardData.flavorText.text;

        }
    }
}
