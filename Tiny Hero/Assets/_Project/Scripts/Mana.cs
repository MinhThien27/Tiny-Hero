using System;
using System.Collections;
using UnityEngine;

public class Mana : MonoBehaviour
{
    [SerializeField] private int maxMana = 100;
    [SerializeField] public int currentMana { get; private set; }
    public event Action<int, int> OnManaChanged;

    private Coroutine regenCoroutine;
    private bool isRegenerating = false;
    private void Awake()
    {
        currentMana = maxMana;
        PublishManaPercent();
    }
    private void Start()
    {
        // Start regenerating mana every second with 1 mana per second
        StartManaRegen(1f, 1); 
    }
    public bool HasEnoughMana(int amount)
    {
        return currentMana >= amount;
    }
    public bool IsFullMana()
    {
        return currentMana >= maxMana;
    }
    public void RestoreMana(int amount)
    {
        if (currentMana < maxMana)
        {
            currentMana += amount;
            
            PublishManaPercent();
        }
        else
        {
            Debug.LogWarning("Mana is already full!");
        }
    }
    public void UseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            PublishManaPercent();
        }
        else
        {
            Debug.LogWarning("Not enough mana!");
        }
    }
    private void PublishManaPercent()
    {
        OnManaChanged?.Invoke(currentMana, maxMana);
    }
    public void RegenManaPerSecond(int amount)
    {
        if (currentMana >= maxMana) return;
        currentMana += amount;
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
        PublishManaPercent();
    }
    public void StartManaRegen(float interval, int amount)
    {
        if (isRegenerating) return;
        regenCoroutine = StartCoroutine(ManaRegenCoroutine(interval, amount));
    }

    public void StopManaRegen()
    {
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
            isRegenerating = false;
        }
    }

    private IEnumerator ManaRegenCoroutine(float interval, int amount)
    {
        isRegenerating = true;
        while (true)
        {
            if (currentMana < maxMana)
            {
                RegenManaPerSecond(amount);
            }
            yield return new WaitForSeconds(interval);
        }
    }

}
