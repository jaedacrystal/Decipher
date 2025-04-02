using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour
{
    public void Play()
    {
        GetComponent<Animator>().Play("FadeOut");
    }

    public void SingleplayerLoadNext()
    {
        SceneManager.LoadScene("Desktop", LoadSceneMode.Single);
    }
}
