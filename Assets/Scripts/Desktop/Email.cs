using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Email : MonoBehaviour
{
    public GameObject email;
    public GameObject notif;
    public GameObject text;
    public TextReveal textReveal;
    public LeanTweenUIManager dialogue;

    void Start()
    {
        email.gameObject.SetActive(false);
    }

    public void showEmail()
    {
        if (textReveal.IsFinished)
        {
            notif.gameObject.SetActive(false);
            text.gameObject.SetActive(false);

            LeanTween.cancel(dialogue.gameObject);
            dialogue.PlayEndAnimation();

            LeanTween.delayedCall(dialogue.endDuration + dialogue.endDelay, () =>
            {
                email.gameObject.SetActive(true);
            });
        }
    }

    public void hideEmail()
    {
        SoundFX.Play("Click");
        email.gameObject.SetActive(false);
        notif.gameObject.SetActive(false);
    }
}
