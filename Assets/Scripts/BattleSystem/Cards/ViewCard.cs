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
    public CardDisplay cardDisplay;
    private CardDisplay cardData;
    public TextMeshProUGUI desc;
    public GameObject cardDesc;

    public static GameObject GlobalDescPrompt;
    public static TextMeshProUGUI GlobalDescText;

    public bool isClicked;

    private static ViewCard currentlyViewedCard;

    private void Start()
    {
        transformObject = GetComponent<RectTransform>();
        originalSiblingIndex = transform.GetSiblingIndex();

        if (initialScale == 0f)
            initialScale = transform.localScale.x;

        cardData = GetComponent<CardDisplay>();

        if (GlobalDescPrompt == null)
        {
            GlobalDescPrompt = GameObject.Find("DescriptionPrompt");
            if (GlobalDescPrompt != null)
            {
                GlobalDescText = GlobalDescPrompt.GetComponentInChildren<TextMeshProUGUI>();
                GlobalDescPrompt.SetActive(false);
            }
            else
            {
                Debug.LogError("Could not find GameObject named 'DescriptionPrompt' in the scene.");
            }
        }
    }

    public void cardClicked()
    {
        cardData = GetComponent<CardDisplay>();

        if (isClicked)
        {
            PerformResetCard(true);
        }
        else
        {
            PerformViewCard();
        }
    }
    private void PerformViewCard()
    {
        if (currentlyViewedCard != null && currentlyViewedCard != this)
        {
            currentlyViewedCard.PerformResetCard(false);
        }

        currentlyViewedCard = this;

        transform.DOKill();
        originalLocalPosition = transformObject.localPosition;
        Vector3 targetPosition = new Vector3(0, 600f, 0);
        transform.DOScale(scale, transitionSpeed);
        transform.DOLocalMove(targetPosition, transitionSpeed);

        transform.SetAsLastSibling();
        isClicked = true;

        if (GlobalDescPrompt != null)
            GlobalDescPrompt.SetActive(true);

        if (cardData != null && cardData.card != null && GlobalDescText != null)
            GlobalDescText.text = cardData.card.flavorTxt;
    }

    private void PerformResetCard(bool animateReset)
    {
        ResetCard(animateReset, true);
    }

    private void ResetCard(bool animate = true, bool playEndAnim = true)
    {
        transform.DOKill();

        if (animate)
        {
            transform.DOLocalMove(originalLocalPosition, transitionSpeed);
            transform.DOScale(initialScale, transitionSpeed);
        }
        else
        {
            transform.localPosition = originalLocalPosition;
            transform.localScale = Vector3.one * initialScale;
        }

        transform.SetSiblingIndex(originalSiblingIndex);
        isClicked = false;

        if (playEndAnim && cardDisplay != null && cardDisplay.tween != null)
        {
            cardDisplay.tween.PlayEndAnimation();
        }

        if (currentlyViewedCard == this)
        {
            currentlyViewedCard = null;
            if (GlobalDescPrompt != null)
            {
                GlobalDescPrompt.SetActive(false);
            }
        }
    }
}