using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform weaponHolderRight;
    public Transform weaponHolderLeft;

    private GameObject currentWeapon;

    public GameObject EquipWeapon(WeaponSO weaponData)
    {
        if (weaponData.itemPrefab == null)
        {
            Debug.LogError("Missing weapon prefab in WeaponSO: " + weaponData.name);
            return null;
        }

        UnequipCurrentWeapon();

        WeaponType type = weaponData.weaponType;
        Transform holder = GetHolder(type);
        currentWeapon = Instantiate(weaponData.itemPrefab, holder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        SetupEquippedWeapon(currentWeapon);

        Debug.Log("Equipped new weapon: " + currentWeapon.name);
        return currentWeapon;
    }

    private void UnequipCurrentWeapon()
    {
        if (currentWeapon == null) return;

        //Destroy(currentWeapon);
        ReturnWeaponToInventory();

        currentWeapon = null;
    }

    private void ReturnWeaponToInventory()
    {
        if (currentWeapon == null) return;
        WeaponPickup collider = currentWeapon.GetComponentInChildren<WeaponPickup>();
        WeaponSO weaponData = currentWeapon.GetComponentInChildren<WeaponPickup>()?.weaponData;
        if (collider != null)
        {
            Debug.Log("Equipping weapon with SO: " + collider.weaponData.name);
        }

        if (weaponData != null)
        {
            InventoryHolder inventory = GetComponent<InventoryHolder>();
            if (inventory != null && inventory.inventoryData.CanAddItem(weaponData))
            {
                inventory.AddItem(weaponData);
                Debug.Log("Returning weapon to inventory with SO: " + weaponData.name);
            }
            else
            {
                Debug.LogWarning("Cannot return weapon to inventory. Inventory full or not found.");
            }
        }
        else
        {
            Debug.LogWarning("Current weapon does not have WeaponPickup or WeaponSO reference.");
        }
        Destroy(currentWeapon);
        currentWeapon = null;
    }

    private void SetupEquippedWeapon(GameObject weapon)
    {
        Rigidbody rb = weapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        foreach (Collider col in weapon.GetComponentsInChildren<Collider>())
        {
            if (col is BoxCollider) col.enabled = false;
            else if (col is CapsuleCollider)
            {
                col.enabled = true;
                col.isTrigger = true;
            }
        }
    }

    private Transform GetHolder(WeaponType type)
    {
        return type == WeaponType.Bow ? weaponHolderLeft : weaponHolderRight;
    }
}
