using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public GameObject Player;
    public float maxHealth = 10;
    public float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Enemy") {
            TakeDamage(2);
        }
    }

    public void TakeDamage (int dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            Destroy(Player);
        }
    }
}
