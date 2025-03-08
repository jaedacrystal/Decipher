using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ComputerPopUp : MonoBehaviour
{
    [SerializeField] public Light2D computerLight;
    [SerializeField] public Collider2D lightTrigger;
    [SerializeField] public Collider2D popupTrigger;
    [SerializeField] public Canvas canvas;

    [SerializeField] public LevelLoader start;
    private bool isPlayerInside = false;

    void Start()
    {
        computerLight.gameObject.SetActive(false);
        canvas.gameObject.SetActive(false);
        start = FindObjectOfType<LevelLoader>();
    }

    private void Update()
    {
        if (isPlayerInside && Input.GetKeyDown("space"))
        {
            start.LoadNextScene();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LevelLoader start = FindObjectOfType<LevelLoader>();

        if (collision.CompareTag("Player"))
        {
            if(collision.IsTouching(lightTrigger))
            {
                computerLight.gameObject.SetActive(true);
            }

            if (collision.IsTouching(popupTrigger))
            {
                canvas.gameObject.SetActive(true);
                isPlayerInside = true;
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!collision.IsTouching(popupTrigger))
            {
                canvas.gameObject.SetActive(false);
            }

        }
    }
}
