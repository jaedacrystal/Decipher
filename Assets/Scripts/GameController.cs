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

            Screen.SetResolution(1920, 1080, true);
            Application.targetFrameRate = 25;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
