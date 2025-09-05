using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public interface IDamageable
{
    bool CanTakeDamage { get; }
    void TakeDamage(int damage);
}

public class Health : MonoBehaviour, IDamageable
{
    [Header("Config")]
    [SerializeField] private int maxHealth = 100;
    public int MaxHealth => maxHealth;

    public int CurrentHealth { get; private set; }

    public GameObject gethitEffect;

    public bool IsDead => CurrentHealth <= 0;
    public virtual bool CanTakeDamage => !IsDead;
    public bool IsTakeDamaged { get; private set; }

    [Header("Events")]
    [SerializeField] private UnityEvent OnDeath;
    public event Action<int, int> OnHealthChanged; // (current, max)

    protected virtual void Awake()
    {
        CurrentHealth = maxHealth;
    }

    protected void Start()
    {
        PublishHealthPercent();
    }

    public virtual void TakeDamage(int damage)
    {
        if (!CanTakeDamage) return;

        SetHealth(CurrentHealth - damage);

        if (gethitEffect != null)
            Instantiate(gethitEffect, transform.position, Quaternion.identity, transform);

        IsTakeDamaged = true;
        StartCoroutine(ResetDamageFlag(0.5f));

        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {CurrentHealth}/{maxHealth}");
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        SetHealth(CurrentHealth + amount);
    }

    public void SetHealth(int value, bool triggerEvents = true)
    {
        CurrentHealth = Mathf.Clamp(value, 0, maxHealth);

        if (triggerEvents)
        {
            PublishHealthPercent();

            if (IsDead)
                Die();
        }
    }

    private void PublishHealthPercent()
    {
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private void Die()
    {
        CurrentHealth = 0;
        OnDeath?.Invoke();
    }

    private IEnumerator ResetDamageFlag(float delay)
    {
        yield return new WaitForSeconds(delay);
        IsTakeDamaged = false;
    }

    public bool IsFullHealth()
    {
        return CurrentHealth >= maxHealth;
    }
}
