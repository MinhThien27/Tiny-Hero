using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Enemy))]
public class Explosion : MonoBehaviour
{
    [SerializeField] UnityEvent OnExplosion; 
    [SerializeField] int damage = 1;
    [SerializeField] Enemy enemy;

    private void Awake()
    {
        if (enemy == null)
        {
            enemy = GetComponent<Enemy>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (enemy.IsExplosion) return;

        if (other.CompareTag("Player"))
        {
            enemy.IsExplosion = true;

            other.GetComponent<PlayerHealth>().TakeDamage(damage);
            Debug.Log("Player hit by explosion for " + damage + " damage.");
            OnExplosion?.Invoke();
        }
    }
}
