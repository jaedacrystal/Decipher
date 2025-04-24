using DG.Tweening;
using TMPro;
using UnityEngine;

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

    private static ViewCard currentlyViewedCard;

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
            ResetCard();
        }
        else
        {
            if (currentlyViewedCard != null && currentlyViewedCard != this)
            {
                currentlyViewedCard.ResetCard();
            }

            currentlyViewedCard = this;
            originalLocalPosition = transformObject.localPosition;

            Vector3 targetPosition = new Vector3(0, 600f, 0);
            transform.DOScale(scale, transitionSpeed);
            transform.DOLocalMove(targetPosition, transitionSpeed);

            transform.SetAsLastSibling();
            isClicked = true;

            cardDisplay.descPrompt.gameObject.SetActive(true);
            desc.text = cardData.descText.text;
        }
    }

    private void ResetCard()
    {
        transform.DOLocalMove(originalLocalPosition, transitionSpeed);
        transform.DOScale(initialScale, transitionSpeed);

        transform.SetSiblingIndex(originalSiblingIndex);
        isClicked = false;

        cardDisplay.descPrompt.gameObject.SetActive(false);
        cardDisplay.tween.PlayEndAnimation();

        if (currentlyViewedCard == this)
        {
            currentlyViewedCard = null;
        }
    }
}
