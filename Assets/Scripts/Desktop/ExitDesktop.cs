using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitDesktop : MonoBehaviour
{
    public Image img;
    public Sprite newImg;
    public Sprite oldImg;

    void Start()
    {
        oldImg = img.sprite;
    }

    public void ExitGame()
    {
        SoundFX.Play("PowerButton");

        img.sprite = newImg;
        StartCoroutine(RevertImage());

        Invoke("Exit", 0.6f);
    }

    private IEnumerator RevertImage()
    {
        yield return new WaitForSeconds(0.3f);
        img.sprite = oldImg;
    }

    private void Exit()
    {
        Application.Quit();

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
