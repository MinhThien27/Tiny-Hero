using Unity.VisualScripting;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    public WeaponSO weaponData;
    PlayerController playerController;

    [Header("For Ranged Weapons")]
    public GameObject arrowPrefab;
    public Transform firePosition;
    public float arrowForce = 20f;

    public bool isAlreadyHit = false;
    public bool isEquipped = false;

    private void Start()
    {
        if (firePosition == null)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player != null)
            {
                firePosition = player.Find("FirePos");
                if (firePosition == null)
                {
                    Debug.LogWarning("Cant find Fire position in Player!");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("is weapon equipped: " + isEquipped);
        if (isAlreadyHit || !isEquipped) return;

        if (weaponData.weaponType == WeaponType.Bow)
            return;

        if (other.CompareTag("Enemy"))
        {
            isAlreadyHit = true;
            Debug.Log("Hit Enemy: " + other.name);

            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(weaponData.damage);
            }
        }
    }

    public void ResetHit()
    {
        isAlreadyHit = false;
    }

    public void Fire()
    {
        if (weaponData == null)
        {
            Debug.LogWarning("Weapon data not assigned!");
            return;
        }

        if (weaponData.weaponType != WeaponType.Bow)
        {
            Debug.LogWarning("Fire() is only for ranged weapons!");
            return;
        }

        if (arrowPrefab == null)
        {
            Debug.LogWarning("Arrow prefab not assigned!");
            return;
        }

        if (firePosition == null)
        {
            Debug.LogWarning("Fire position not assigned!");
            return;
        }

        GameObject arrow = Instantiate(arrowPrefab, firePosition.position, firePosition.rotation);

        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePosition.forward * arrowForce;
        }
    }
}
