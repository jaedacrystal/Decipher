using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isDragging;

    private GameObject graveyard;
    private TextMeshProUGUI errorText;

    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector3 originalPosition;

    private CardDisplay cardDisplay;
    private CardManager cardManager;
    private RectTransform playArea;
    private Discard discard;

    private ViewCard viewCard;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        cardDisplay = GetComponent<CardDisplay>();
        cardManager = FindObjectOfType<CardManager>();
        playArea = GameObject.Find("PlayArea").GetComponent<RectTransform>();
        discard = GetComponent<Discard>();
        graveyard = GameObject.Find("Graveyard");
        errorText = GameObject.Find("BandwidthErrorText").GetComponent<TextMeshProUGUI>();

        viewCard = GetComponent<ViewCard>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (viewCard != null && viewCard.isClicked == true) return;

        isDragging = true;

        if (isDragging)
        {
            canvasGroup.blocksRaycasts = false;
            originalParent = transform.parent;
            originalPosition = transform.localPosition;
            transform.SetParent(originalParent.parent);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (viewCard != null && viewCard.isClicked == true) return;

        Vector3 worldPoint;
        RectTransform rectTransform = GetComponent<RectTransform>();
        Canvas canvas = GetComponentInParent<Canvas>();

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, canvas.worldCamera, out worldPoint))
        {
            transform.position = worldPoint;
        }

        transform.SetAsLastSibling();
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (viewCard != null && viewCard.isClicked == true) return;

        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        if (IsOverPlayArea(eventData))
        {
            PlayCard();
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }

    private bool IsOverPlayArea(PointerEventData eventData)
    {
        if (viewCard != null && viewCard.isClicked == true) return false;
        return playArea != null && RectTransformUtility.RectangleContainsScreenPoint(playArea, eventData.position, GetComponentInParent<Canvas>().worldCamera);
    }


    private void PlayCard()
    {
        if (viewCard != null && viewCard.isClicked == true) return;
        if (cardDisplay == null || cardDisplay.card == null) return;

        PlayerStats playerStats = cardManager != null && cardManager.player != null
            ? cardManager.player.GetComponent<PlayerStats>()
            : FindLocalPlayerStats();

        GameObject opponent = cardManager != null
            ? cardManager.opponent
            : FindOpponentGameObject();

        if (playerStats != null && playerStats.CanPlayCard(cardDisplay.card.bandwidth))
        {
            playerStats.UseBandwidth(cardDisplay.card.bandwidth);

            cardDisplay.card.ApplyEffect(playerStats.gameObject, opponent);

            MoveToGraveyard(this);
        }
        else
        {
            ShowError("Not enough bandwidth!");
            Invoke("ReturnToOriginalPosition", 0.5f);
        }
    }


    private PlayerStats FindLocalPlayerStats()
    {
        foreach (PlayerStats stats in FindObjectsOfType<PlayerStats>())
        {
            PhotonView view = stats.GetComponent<PhotonView>();
            if (view == null || view.IsMine) return stats;
        }
        return null;
    }

    private GameObject FindOpponentGameObject()
    {
        foreach (PlayerStats stats in FindObjectsOfType<PlayerStats>())
        {
            PhotonView view = stats.GetComponent<PhotonView>();
            if (view != null && !view.IsMine) return stats.gameObject;
        }
        return null;
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
