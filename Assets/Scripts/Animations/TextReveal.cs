using System.Collections;
using TMPro;
using UnityEngine;

public class TextReveal : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float revealSpeed;
    public float punctuationPause;

    private string soundName;
    private Coroutine revealCoroutine;
    public bool isRevealing;

    public bool IsFinished => !isRevealing;

    private void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    public void SetTypingSound(string sound)
    {
        soundName = sound;
    }

    public void StartReveal(string text)
    {
        if (revealCoroutine != null)
        {
            StopCoroutine(revealCoroutine);
        }

        textComponent.text = "";
        isRevealing = true;
        revealCoroutine = StartCoroutine(RevealText(text));
    }

    public void SkipReveal(string fullText)
    {
        if (isRevealing)
        {
            StopCoroutine(revealCoroutine);
            textComponent.text = fullText;
            isRevealing = false;
        }
    }

    private IEnumerator RevealText(string fullText)
    {
        for (int i = 0; i <= fullText.Length; i++)
        {
            textComponent.text = fullText.Substring(0, i);

            if (i > 0)
            {
                PlayTypingSound();

                if (Punctuation(fullText[i - 1]))
                {
                    yield return new WaitForSeconds(punctuationPause);
                }
                else
                {
                    yield return new WaitForSeconds(revealSpeed);
                }
            }
        }

        isRevealing = false;
    }

    private void PlayTypingSound()
    {
        if (!string.IsNullOrEmpty(soundName))
        {
            SoundFX.Play(soundName);
        }
    }

    private bool Punctuation(char c)
    {
        return c == '.' || c == ',' || c == '!' || c == '?';
    }
}
