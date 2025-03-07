//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class CardDisplay : MonoBehaviour
//{
//    public Cards card;

//    public Image img;

//    public TextMeshProUGUI nameText;
//    public TextMeshProUGUI descText;

//    public TextMeshProUGUI bandwidth;


//    void Start()
//    {
//        img.sprite = card.artwork;
//        nameText.text = card.cardName;
//        descText.text = card.desc;

//        bandwidth.text = card.bandwidth.ToString();
//    }

//}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Cards card;

    public Image img;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;

    public TextMeshProUGUI bandwidth;


    public void SetCard(Cards cardData)
    {
        if (cardData == null) return;

        card = cardData;

        img.sprite = card.artwork;
        nameText.text = card.cardName;
        descText.text = card.desc;
        bandwidth.text = card.bandwidth.ToString();
    }
}



