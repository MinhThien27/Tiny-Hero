using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ManaHUD : MonoBehaviour
{
    public Image manaBarImage;
    public Mana playerMana;

    private void Start()
    {
        playerMana.OnManaChanged += UpdateManaBar;
    }

    private void OnDestroy()
    {
        playerMana.OnManaChanged -= UpdateManaBar;
    }

    public void UpdateManaBar(int currentHealth, int maxHealth)
    {
        float fillAmount = (float)currentHealth / maxHealth;
        manaBarImage.fillAmount = fillAmount;
    }
}
