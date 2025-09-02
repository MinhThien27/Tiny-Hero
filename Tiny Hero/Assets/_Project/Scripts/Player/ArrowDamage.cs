using System.Collections;
using UnityEngine;

public class ArrowDamage : MonoBehaviour
{
    public WeaponSO weaponData;

    private void Start()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit Enemy: " + other.name);

            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(weaponData.damage);
            }

            Destroy(gameObject);
        }
    }
}
