using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PutCardOnTop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public float initialScale;
    [SerializeField] public float finalScale;
    [SerializeField] public float transitionSpeed;

    private RectTransform transformObject;
    private int originalSiblingIndex;
    private HorizontalLayoutGroup parentLayout;
    [HideInInspector] public Vector3 originalLocalPosition;

    private bool isClicked;

    private void Start()
    {
        transformObject = GetComponent<RectTransform>();
        originalSiblingIndex = transform.GetSiblingIndex();
        parentLayout = GetComponentInParent<HorizontalLayoutGroup>();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (parentLayout) parentLayout.enabled = false;

        transformObject.DOScale(new Vector3(finalScale, finalScale, finalScale), transitionSpeed);
        transform.SetAsLastSibling();
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (!isClicked)
        {
            transformObject.DOScale(new Vector3(initialScale, initialScale, initialScale), transitionSpeed);
            transform.SetSiblingIndex(originalSiblingIndex);

            if (parentLayout) parentLayout.enabled = true;
        }
    }

    public void cardClicked()
    {
        if (isClicked)
        {
            transform.DOLocalMove(originalLocalPosition, 0.25f);
            isClicked = false;
        }
        else
        {
            originalLocalPosition = transformObject.localPosition;

            Vector3 targetPosition = new Vector3(0, 500f, 0);
            transform.DOLocalMove(targetPosition, 0.25f);
            isClicked = true;
        }

    }
}
