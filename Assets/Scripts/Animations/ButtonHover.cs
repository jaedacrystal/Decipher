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
    public bool buttonHover = true; 

    private RectTransform transformObject;

    private void Start()
    {
        transformObject = GetComponent<RectTransform>();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (buttonHover == true) 
        {
            transformObject.DOScale(new Vector3(finalScale, finalScale, finalScale), transitionSpeed);
        } else
        {
            return;
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (buttonHover == true)
        {
            transformObject.DOScale(new Vector3(initialScale, initialScale, initialScale), transitionSpeed);
        } else
        {
            return;
        }

    }
}
