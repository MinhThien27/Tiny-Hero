using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Data")]
    public WeaponSO weaponData;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        InventoryHolder inventoryHolder = other.GetComponent<InventoryHolder>();
        if (inventoryHolder == null)
        {
            Debug.LogWarning("InventoryHolder not found on player.");
            return;
        }

        if (weaponData == null)
        {
            Debug.LogError("WeaponPickup missing WeaponSO reference on " + gameObject.name);
            return;
        }

        // Add weapon to inventory
        inventoryHolder.AddItem(weaponData);
        Debug.Log("Picked up weapon: " + weaponData.itemName);

        // Destroy or deactivate the pickup parent's object
        GameObject parent = transform.parent != null ? transform.parent.gameObject : gameObject;
        Destroy(parent);
    }
}
