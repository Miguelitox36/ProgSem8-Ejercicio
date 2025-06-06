using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start() => currentHealth = maxHealth;

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            GameManager.Instance.GameOver();
            Destroy(gameObject);
        }
    }
}