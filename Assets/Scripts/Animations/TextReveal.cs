using System.Collections;
using TMPro;
using UnityEngine;

public class TextReveal : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float revealSpeed;
    public float punctuationPause;
    private Coroutine revealCoroutine;

    private void Start()
    {
        textComponent.text = "";
    }

    public void StartReveal(string text)
    {
        if (revealCoroutine != null)
        {
            StopCoroutine(revealCoroutine);
        }

        textComponent.text = "";
        revealCoroutine = StartCoroutine(RevealText(text));
    }

    private IEnumerator RevealText(string fullText)
    {
        for (int i = 0; i <= fullText.Length; i++)
        {
            textComponent.text = fullText.Substring(0, i);

            if (i > 0 && Punctuation(fullText[i - 1]))
            {
                yield return new WaitForSeconds(punctuationPause);
            }
            else
            {
                yield return new WaitForSeconds(revealSpeed);
            }
        }
    }

    private bool Punctuation(char c)
    {
        return c == '.' || c == ',' || c == '!' || c == '?';
    }
}
