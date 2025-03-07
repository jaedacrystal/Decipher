using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public float initialScale;
    [SerializeField] public float finalScale;
    [SerializeField] public float transitionSpeed;

    private RectTransform transformObject;

    private void Start()
    {
        transformObject = GetComponent<RectTransform>();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        transformObject.DOScale(new Vector3(finalScale, finalScale, finalScale), transitionSpeed);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        transformObject.DOScale(new Vector3(initialScale, initialScale, initialScale), transitionSpeed);
    }
}
