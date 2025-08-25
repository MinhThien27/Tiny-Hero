using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;

public interface IDamageable
{
    bool CanTakeDamage { get; }
    void TakeDamage(int damage);
}
public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] public int currentHealth { get; private set; }

    public GameObject gethitEffect;

    public bool isDeath => currentHealth <= 0; 
    public virtual bool CanTakeDamage => !isDeath;
    public bool isTakeDamaged {  get; private set; }

    [Header("Events")]
    [SerializeField] private UnityEvent OnDeath;
    [SerializeField] public event Action<int, int> OnHealthChanged;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

     protected void Start()
    {
        PublishHealthPercent();
    }

    public virtual void TakeDamage(int damage)
    {
        if (!CanTakeDamage) return;

        currentHealth -= damage;
        Instantiate(gethitEffect, transform.position, Quaternion.identity, transform);

        isTakeDamaged = true;

        PublishHealthPercent();

        if (isDeath)
        {
            Die();
        }
        StartCoroutine(ResetDamageFlag(0.5f));
    }

    public bool IsFullHealth()
    {
        return currentHealth >= maxHealth;
    }

    public void Heal(int amount)
    {
        if (isDeath) return;
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        PublishHealthPercent();
    }

    private void PublishHealthPercent()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Die()
    {
        currentHealth = 0;
        OnDeath?.Invoke();
    }

    private IEnumerator ResetDamageFlag(float delay)
    {
        yield return new WaitForSeconds(delay);
        isTakeDamaged = false;
    }
}