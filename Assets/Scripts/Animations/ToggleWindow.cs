using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ToggleWindow : MonoBehaviour
{
    public GameObject window;

    LeanTweenUIManager tween;
    PromptDialogue dialogue;

    private void Start()
    {
        tween = GetComponent<LeanTweenUIManager>();
        dialogue = GetComponent<PromptDialogue>();
        window.SetActive(false);
    }

    public void ToggleMenu()
    {
        bool isMenuActive = window.activeSelf;

        if (isMenuActive && tween.playEndAnimation)
        {
            tween.PlayEndAnimation(() => window.SetActive(false));
        }
        else
        {
            window.SetActive(true);
            tween.PlayAnimation();
        }

        SoundFX.Play("Click");
    }
}
