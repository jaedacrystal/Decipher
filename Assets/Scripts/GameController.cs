using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            Screen.SetResolution(Screen.width, Screen.height, true);
            Application.targetFrameRate = 60;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
