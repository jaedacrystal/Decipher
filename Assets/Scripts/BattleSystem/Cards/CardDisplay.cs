using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Cards card;

    public Image img;

    [Header("Text")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI flavorText;
    public TextMeshProUGUI bandwidth;

    public LeanTweenUIManager tween;
    public GameObject descPrompt;

    private void Start()
    {
        descPrompt = GameObject.Find("DescriptionPrompt");
        //tween = descPrompt.GetComponent<LeanTweenUIManager>();
    }

    public void SetCard(Cards cardData)
    {
        if (cardData == null) return;

        card = cardData;

        flavorText.text = card.flavorTxt;
        img.sprite = card.artwork;
        nameText.text = card.cardName;
        descText.text = card.desc;
        bandwidth.text = card.bandwidth.ToString();
    }
}


