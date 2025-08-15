using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Health : MonoBehaviour 
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] int currentHealth;
    public bool isDeath => currentHealth <= 0; 

    [SerializeField] FloatEventChannel playerHealthChannel;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        PublishHealthPercent();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        PublishHealthPercent();
    }

    private void PublishHealthPercent()
    {
        if (playerHealthChannel != null)
        {
            playerHealthChannel.Invoke(currentHealth / (float)maxHealth);
        }
    }
}
