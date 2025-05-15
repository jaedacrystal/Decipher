using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewCard : MonoBehaviour
{
    [Header("Scale Settings")]
    public float initialScale = 1f;
    public float scale = 1.5f;
    public float transitionSpeed = 0.25f;

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

    public static ViewCard currentlyViewedCard;

    private void Start()
    {
        transformObject = GetComponent<RectTransform>();
        originalSiblingIndex = transform.GetSiblingIndex();

        if (initialScale == 0f)
            initialScale = transform.localScale.x;

        cardData = GetComponent<CardDisplay>();
        cardDisplay = FindObjectOfType<CardDisplay>();

        cardDesc = GameObject.Find("DescriptionPrompt");
        desc = cardDesc.GetComponentInChildren<TextMeshProUGUI>();

        if (cardDisplay != null && cardDisplay.descPrompt != null)
            cardDisplay.descPrompt.SetActive(false);
    }

    public void cardClicked()
    {
        if (currentlyViewedCard != null && currentlyViewedCard != this)
        {
            currentlyViewedCard.ResetCard();
        }

        if (isClicked)
        {
            ResetCard();
        }
        else
        {
            originalLocalPosition = transform.localPosition;
            originalSiblingIndex = transform.GetSiblingIndex();

            Vector3 targetPosition = new Vector3(0, 600f, 0);
            transform.DOScale(scale, transitionSpeed);
            transform.DOLocalMove(targetPosition, transitionSpeed);

            transform.SetAsLastSibling();
            isClicked = true;
            currentlyViewedCard = this;

            if (cardDisplay != null && cardDisplay.descPrompt != null)
                cardDisplay.descPrompt.SetActive(true);

            if (cardData != null && cardData.card != null)
                desc.text = cardData.card.desc;
            else
                desc.text = "No description available.";
        }
    }

    public void ResetCard()
    {
        transform.DOLocalMove(originalLocalPosition, transitionSpeed);
        transform.DOScale(initialScale, transitionSpeed);

        transform.SetSiblingIndex(originalSiblingIndex);
        isClicked = false;

        if (cardDisplay != null && cardDisplay.tween != null)
            cardDisplay.tween.PlayEndAnimation();

        if (currentlyViewedCard == this)
            currentlyViewedCard = null;
    }
}