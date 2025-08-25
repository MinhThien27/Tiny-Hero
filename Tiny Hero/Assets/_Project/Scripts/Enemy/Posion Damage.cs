using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosionDamage : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float delayDamage;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(ApplyPoisonDamage(other.GetComponent<PlayerHealth>()));
        }
    }

    private IEnumerator ApplyPoisonDamage(PlayerHealth playerHealth)
    {
        while (true)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            yield return new WaitForSeconds(delayDamage);
        }
    }
}
