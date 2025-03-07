using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Cards : ScriptableObject
{
    public Sprite artwork;

    public string cardName;
    [TextArea(3, 3)]
    public string desc;
    
    public int bandwidth; 
}
