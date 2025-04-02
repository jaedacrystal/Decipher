using TMPro;
using UnityEngine;

public class Text : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextReveal textReveal;
    public string typingSoundName;

    void Start()
    {
        textReveal.StartReveal(text.text);
        textReveal.SetTypingSound(typingSoundName);
    }
}
