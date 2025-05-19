using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent ( typeof ( CanvasGroup ), typeof ( CardDisplay ) )]
public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
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
    private Canvas canvas;

    private void Awake () {
        canvasGroup = GetComponent<CanvasGroup> ();
        cardDisplay = GetComponent<CardDisplay> ();
        discard = GetComponent<Discard> ();
        viewCard = GetComponent<ViewCard> ();
        canvas = GetComponentInParent<Canvas> ();
    }

    private void Start () {
        // These must be present in the scene and correctly named
        GameObject playAreaGO = GameObject.Find ( "PlayArea" );
        GameObject graveyardGO = GameObject.Find ( "Graveyard" );
        GameObject errorTextGO = GameObject.Find ( "BandwidthErrorText" );

        if ( playAreaGO != null )
            playArea = playAreaGO.GetComponent<RectTransform> ();

        graveyard = graveyardGO;
        errorText = errorTextGO != null ? errorTextGO.GetComponent<TextMeshProUGUI> () : null;

        cardManager = FindObjectOfType<CardManager> ();
    }

    public void OnBeginDrag ( PointerEventData eventData ) {
        if ( viewCard != null && viewCard.isClicked )
            return;

        isDragging = true;
        canvasGroup.blocksRaycasts = false;
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        transform.SetParent ( originalParent.parent ); // Bring to top
    }

    public void OnDrag ( PointerEventData eventData ) {
        if ( viewCard != null && viewCard.isClicked )
            return;
        if ( canvas == null || canvas.worldCamera == null )
            return;

        RectTransform rectTransform = GetComponent<RectTransform> ();
        if ( RectTransformUtility.ScreenPointToWorldPointInRectangle ( rectTransform, eventData.position, canvas.worldCamera, out Vector3 worldPoint ) ) {
            transform.position = worldPoint;
            transform.SetAsLastSibling ();
        }
    }

    public void OnEndDrag ( PointerEventData eventData ) {
        if ( viewCard != null && viewCard.isClicked )
            return;

        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        if ( IsOverPlayArea ( eventData ) ) {
            PlayCard ();
        } else {
            ReturnToOriginalPosition ();
        }
    }

    private bool IsOverPlayArea ( PointerEventData eventData ) {
        if ( playArea == null || canvas == null || canvas.worldCamera == null )
            return false;

        return RectTransformUtility.RectangleContainsScreenPoint ( playArea, eventData.position, canvas.worldCamera );
    }

    private void PlayCard () {
        if ( cardDisplay?.card == null )
            return;

        PlayerStats playerStats = cardManager.playerStats;

        if ( playerStats != null && playerStats.CanPlayCard ( cardDisplay.card.bandwidth ) ) {
            playerStats.UseBandwidth ( cardDisplay.card.bandwidth );

            cardDisplay.card.ApplyEffect ( cardManager.opponentStats, cardManager.opponentHealth, cardManager.playerStats, cardManager.playerHealth, cardDisplay.card.target );

            cardManager.effectText.text = cardDisplay.card.cardName + " Played";
            MoveToGraveyard ( this );

        } else {
            ShowError ( "Not enough bandwidth!" );
            Invoke ( nameof ( ReturnToOriginalPosition ), 0.5f );
        }
    }

    private void MoveToGraveyard ( CardDrag card ) {
        if ( graveyard == null )
            return;

        card.transform.SetParent ( graveyard.transform, false );
        card.gameObject.SetActive ( false );

        if ( discard != null ) {
            discard.UpdateGraveyardCounter ();
        }
    }

    private void ReturnToOriginalPosition () {
        transform.SetParent ( originalParent );
        transform.DOLocalMove ( originalPosition, 0.3f );
    }

    private void ShowError ( string message ) {
        if ( errorText == null )
            return;

        errorText.text = message;
        errorText.gameObject.SetActive ( true );
        errorText.DOKill ();
        errorText.alpha = 1f;

        errorText.transform.SetAsLastSibling ();

        errorText.DOFade ( 0, 1.5f ).SetDelay ( 1f ).OnComplete ( () => {
            errorText.gameObject.SetActive ( false );
        } );
    }
}