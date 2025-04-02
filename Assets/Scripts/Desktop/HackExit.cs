using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HackExit : MonoBehaviour
{
    public GameObject hidePrompt;
    public GameObject btn;
    public Image folder;
    public Sprite folderChange;
    public TextMeshProUGUI text;

    [HideInInspector] public bool exit;

    private void Start()
    {
        btn.SetActive(false);
    }

    public void ExitPrompt()
    {
        folder.GetComponent<Image>().sprite = folderChange;
        SoundFX.Play("Click");
        hidePrompt.SetActive(false);
    }


    public void Hide()
    {
        SoundFX.Play("Click");

        if (AllSiblingsHidden())
        {
            text.text = "Something's wrong...";
            btn.SetActive(true);
        }
    }

    private bool AllSiblingsHidden()
    {
        Transform parent = hidePrompt.transform.parent;

        foreach (Transform sibling in parent)
        {
            if (sibling.gameObject == hidePrompt) continue;


            if (sibling.gameObject.activeSelf) return false;
        }

        return true;
    }
}