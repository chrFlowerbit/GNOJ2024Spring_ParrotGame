using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class HealthChangedEventArgs : System.EventArgs
{
    public float CurrentHealth { get; }
    public float MaxHealth { get; }

    public HealthChangedEventArgs(float currentHealth, float maxHealth)
    {
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
    }
}
public class HealthManager : MonoBehaviour
{

    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    // Using predefined EventHandler delegate for events
    public event EventHandler<HealthChangedEventArgs> OnHealthChanged;
    public event EventHandler OnDeath;


    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public bool TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log(currentHealth);
        // Invoke the OnHealthChanged event
        OnHealthChanged?.Invoke(this, new HealthChangedEventArgs(currentHealth, maxHealth));

        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Invoke the OnHealthChanged event
        OnHealthChanged?.Invoke(this, new HealthChangedEventArgs(currentHealth, maxHealth));
    }

    private void Die()
    {
        OnDeath?.Invoke(this, EventArgs.Empty);
        // Handle death here - disable character control, play death animation, etc.
        Debug.Log(gameObject.name + " has died.");
        Destroy(gameObject);
    }

    // For debugging purposes
    private void Update()
    {
    
    }






}
