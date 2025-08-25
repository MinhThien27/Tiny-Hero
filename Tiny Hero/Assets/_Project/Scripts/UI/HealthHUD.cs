using UnityEngine;
using UnityEngine.UI;

public class HealthHUD : MonoBehaviour
{
    public Image healthBarImage;
    public PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDestroy()
    {
        playerHealth.OnHealthChanged -= UpdateHealthBar;
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        float fillAmount = (float)currentHealth / maxHealth;
        healthBarImage.fillAmount = fillAmount;
    }
}
