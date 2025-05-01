using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragMultiplayer : MonoBehaviourPunCallbacks, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isDragging;

    private GameObject graveyard;
    private TextMeshProUGUI errorText;

    [Header("Components")]
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector3 originalPosition;

    [Header("Cards")]
    private CardDisplay cardDisplay;
    private MultiCardManager multiCardManager;
    private RectTransform playArea;
    private Discard discard;

    private ViewCard viewCard;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        cardDisplay = GetComponent<CardDisplay>();
        multiCardManager = FindObjectOfType<MultiCardManager>();
        playArea = GameObject.Find("PlayArea")?.GetComponent<RectTransform>();
        discard = GetComponent<Discard>();
        graveyard = GameObject.Find("Graveyard");
        errorText = GameObject.Find("BandwidthErrorText")?.GetComponent<TextMeshProUGUI>();
        viewCard = GetComponent<ViewCard>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (viewCard != null && viewCard.isClicked) return;

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
        if (viewCard != null && viewCard.isClicked) return;

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
        if (viewCard != null && viewCard.isClicked) return;

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
        if (viewCard != null && viewCard.isClicked) return false;
        return playArea != null && RectTransformUtility.RectangleContainsScreenPoint(playArea, eventData.position, GetComponentInParent<Canvas>().worldCamera);
    }

    private void PlayCard()
    {
        if (viewCard != null && viewCard.isClicked) return;
        if (cardDisplay == null || cardDisplay.card == null) return;

        GameObject player = multiCardManager?.myPlayer;
        GameObject opponent = multiCardManager?.enemyPlayer;

        if (player == null || opponent == null)
        {
            Debug.LogError("MultiCardManager failed to assign myPlayer or enemyPlayer!");
            ReturnToOriginalPosition();
            return;
        }

        PlayerStats playerStats = player.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            if (playerStats.CanPlayCard(cardDisplay.card.bandwidth))
            {
                playerStats.UseBandwidth(cardDisplay.card.bandwidth);
                cardDisplay.card.ApplyEffect(player, opponent);
                MoveToGraveyard(this);

                PhotonView.Get(this).RPC("RPC_PlayCardOnOtherClient", RpcTarget.Others,
                    cardDisplay.card.cardName, cardDisplay.card.isSingleplayer,
                    cardDisplay.card.effectValue, cardDisplay.card.target);
            }
            else
            {
                ShowError($"Not enough bandwidth!");
                Invoke("ReturnToOriginalPosition", 0.5f);
            }
        }
        else
        {
            Debug.LogError("PlayerStats component not found on the player GameObject!");
            ReturnToOriginalPosition();
        }
    }

    [PunRPC]
    public void RPC_PlayCardOnOtherClient(string cardName, bool isSingleplayer, int effectValue, TargetType target)
    {
        Debug.Log($"Opponent played: {cardName}");

        if (multiCardManager != null)
        {
            multiCardManager.HandleOpponentPlayedCard(cardName);

            GameObject player = multiCardManager.enemyPlayer;
            GameObject opponent = multiCardManager.myPlayer;

            if (player != null && opponent != null)
            {
                Cards card = multiCardManager.listOfCards.Find(c => c.cardName == cardName);
                if (card != null)
                {
                    if (isSingleplayer)
                    {
                        Health targetHealth = target == TargetType.Player ? player.GetComponent<Health>() : opponent.GetComponent<Health>();
                        PlayerStats targetStats = target == TargetType.Player ? player.GetComponent<PlayerStats>() : opponent.GetComponent<PlayerStats>();
                        card.ApplyEffect(targetHealth.gameObject, opponent);
                    }
                    else
                    {
                        MultiplayerHealth targetHealth = target == TargetType.Player ? player.GetComponent<MultiplayerHealth>() : opponent.GetComponent<MultiplayerHealth>();
                        PlayerStats targetStats = target == TargetType.Player ? player.GetComponent<PlayerStats>() : opponent.GetComponent<PlayerStats>();
                        card.ApplyEffect(targetHealth.gameObject, opponent);
                    }
                }
                else
                {
                    Debug.LogWarning($"Card {cardName} not found in the listOfCards!");
                }
            }
        }
    }

    private void MoveToGraveyard(CardDragMultiplayer card)
    {
        card.transform.SetParent(graveyard.transform, false);
        card.gameObject.SetActive(false);

        if (discard != null)
        {
            discard.UpdateGraveyardCounter();
        }
    }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        transform.DOLocalMove(originalPosition, 0.3f);
    }

    private void ShowError(string message)
    {
        if (errorText == null)
        {
            GameObject errorObj = GameObject.Find("BandwidthErrorText");
            if (errorObj != null)
                errorText = errorObj.GetComponent<TextMeshProUGUI>();
        }

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
