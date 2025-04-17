using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isDragging;

    [SerializeField] private GameObject graveyard;
    private TextMeshProUGUI errorText;

    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector3 originalPosition;

    private CardDisplay cardDisplay;
    private CardManager cardManager;
    private RectTransform playArea;
    private Discard discard;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        cardDisplay = GetComponent<CardDisplay>();
        cardManager = FindObjectOfType<CardManager>();
        playArea = GameObject.Find("PlayArea").GetComponent<RectTransform>();
        discard = GetComponent<Discard>();
        graveyard = GameObject.Find("Graveyard");
        errorText = GameObject.Find("BandwidthErrorText").GetComponent<TextMeshProUGUI>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        canvasGroup.blocksRaycasts = false;
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        transform.SetParent(originalParent.parent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        transform.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        if (IsOverPlayArea())
        {
            PlayCard();
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }

    private bool IsOverPlayArea()
    {
        return playArea != null && RectTransformUtility.RectangleContainsScreenPoint(playArea, Input.mousePosition);
    }

    private void PlayCard()
    {
        if (cardDisplay == null || cardDisplay.card == null) return;

        PlayerStats playerStats = cardManager.player.GetComponent<PlayerStats>();

        if (playerStats != null && playerStats.CanPlayCard(cardDisplay.card.bandwidth))
        {
            playerStats.UseBandwidth(cardDisplay.card.bandwidth);
            cardDisplay.card.ApplyEffect(cardManager.opponent, cardManager.player);

            MoveToGraveyard(this);
        }
        else
        {
            ShowError("Not enough bandwidth!");
            Invoke("ReturnToOriginalPosition", 0.5f);
        }
    }

    private void MoveToGraveyard(CardDrag card)
    {
        card.transform.SetParent(graveyard.transform, false);
        card.gameObject.SetActive(false);
        FindObjectOfType<Discard>().UpdateGraveyardCounter();
    }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        transform.DOLocalMove(originalPosition, 0.3f);
    }

    private void ShowError(string message)
    {
        if (errorText == null) return;

        errorText.text = message;
        errorText.gameObject.SetActive(true);
        errorText.DOKill();
        errorText.alpha = 1f;

        errorText.transform.SetAsLastSibling();

        errorText.DOFade(0, 1.5f).SetDelay(1f).OnComplete(() =>
        {
            errorText.gameObject.SetActive(false);
        });
    }
}
