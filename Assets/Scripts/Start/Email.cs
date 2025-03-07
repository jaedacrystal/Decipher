using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Email : MonoBehaviour
{
    [SerializeField] public GameObject email;
    [SerializeField] public GameObject notif;

    void Start()
    {
        email.gameObject.SetActive(false);
        notif.gameObject.SetActive(true);
    }

    public void showEmail()
    {
        email.gameObject.SetActive(true);
        notif.gameObject.SetActive(false);
    }

    public void hideEmail()
    {
        email.gameObject.SetActive(false);
        notif.gameObject.SetActive(false);
    }
}
