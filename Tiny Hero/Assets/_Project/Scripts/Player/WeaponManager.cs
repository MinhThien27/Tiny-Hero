using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Holders")]
    public Transform weaponHolderRight;
    public Transform weaponHolderLeft;

    public GameObject currentWeaponLeft { get; private set; }
    public GameObject currentWeaponRight { get; private set; }

    #region Equip
    private bool IsSameWeaponEquipped(GameObject equippedWeapon, WeaponSO newWeapon)
    {
        if (equippedWeapon == null) return false;

        var existingData = equippedWeapon.GetComponentInChildren<WeaponCollider>()?.weaponData;
        return existingData == newWeapon;
    }

    public GameObject EquipWeapon(WeaponSO weaponData)
    {
        if (weaponData == null || weaponData.itemPrefab == null)
        {
            Debug.LogError("No prefab for weapon");
            return null;
        }

        WeaponType type = weaponData.weaponType;

        switch (type)
        {
            case WeaponType.Bow:
                UnequipWeapon(currentWeaponLeft);
                UnequipWeapon(currentWeaponRight);
                currentWeaponLeft = InstantiateWeapon(weaponData, weaponHolderLeft);
                return currentWeaponLeft;

            case WeaponType.Shield:
                UnequipWeapon(currentWeaponLeft);
                currentWeaponLeft = InstantiateWeapon(weaponData, weaponHolderLeft);
                return currentWeaponLeft;

            case WeaponType.Sword:
                UnequipWeapon(currentWeaponRight);

                if (currentWeaponLeft != null &&
                    currentWeaponLeft.GetComponentInChildren<WeaponCollider>()?.weaponData.weaponType != WeaponType.Shield)
                {
                    UnequipWeapon(currentWeaponLeft);
                }

                currentWeaponRight = InstantiateWeapon(weaponData, weaponHolderRight);
                return currentWeaponRight;
        }

        Debug.LogWarning("Type not declare: " + type);
        return null;
    }


    public GameObject EquipRightHand(WeaponSO weaponData)
    {
        if (weaponData == null || weaponData.itemPrefab == null)
        {
            Debug.LogError("No prefab for weapon (right hand)");
            return null;
        }
        UnequipWeapon(currentWeaponRight);

        currentWeaponRight = InstantiateWeapon(weaponData, weaponHolderRight);
        return currentWeaponRight;
    }

    public GameObject EquipLeftHand(WeaponSO weaponData)
    {
        if (weaponData == null || weaponData.itemPrefab == null)
        {
            Debug.LogError("No prefab for weapon (left hand)");
            return null;
        }
        UnequipWeapon(currentWeaponLeft);

        currentWeaponLeft = InstantiateWeapon(weaponData, weaponHolderLeft);
        return currentWeaponLeft;
    }
    #endregion

    #region Instantiate + Setup
    private GameObject InstantiateWeapon(WeaponSO weaponData, Transform holder)
    {
        //if (holder.childCount > 0)
        //{
        //    UnequipWeapon(holder.GetChild(0).gameObject);
        //}

        GameObject newWeapon = Instantiate(weaponData.itemPrefab, holder);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = GetWeaponRotation(weaponData.weaponType);

        SetupEquippedWeapon(newWeapon);
        Debug.Log("Equipped new weapon: " + newWeapon.name);

        return newWeapon;
    }

    private Quaternion GetWeaponRotation(WeaponType type)
    {
        return type switch
        {
            WeaponType.Shield => Quaternion.Euler(180f, -90f, -90f),
            WeaponType.Bow => Quaternion.Euler(90f, 0f, 0f),
            _ => Quaternion.identity
        };
    }

    private void SetupEquippedWeapon(GameObject weapon)
    {
        WeaponCollider wc = weapon.GetComponentInChildren<WeaponCollider>();
        if (wc != null) wc.isEquipped = true;

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
    #endregion

    #region Unequip
    public void UnequipWeapon(GameObject weapon)
    {
        if (weapon == null) return;

        WeaponCollider wc = weapon.GetComponentInChildren<WeaponCollider>();
        if (wc != null) wc.isEquipped = false;

        switch (weapon)
        {
            case var wp when wp == currentWeaponRight:
                currentWeaponRight = null;
                break;
            case var wp when wp == currentWeaponLeft:
                currentWeaponLeft = null;
                break;
            default:
                Debug.LogWarning("Weapon to unequip not recognized: " + weapon.name);
                break;
        }

        ReturnWeaponToInventory(weapon);
    }

    public void UnequipRightHand(GameObject weapon, bool isReturnInventory = true)
    {
        if (weapon == null) return;

        WeaponCollider wc = weapon.GetComponentInChildren<WeaponCollider>();
        if (wc != null) wc.isEquipped = false;

        if (isReturnInventory) ReturnWeaponToInventory(weapon);

        if (weapon == currentWeaponRight)
            currentWeaponRight = null;
    }

    public void UnequipLeftHand(GameObject weapon, bool isReturnInventory = true)
    {
        if (weapon == null) return;

        WeaponCollider wc = weapon.GetComponentInChildren<WeaponCollider>();
        if (wc != null) wc.isEquipped = false;

        if (isReturnInventory) ReturnWeaponToInventory(weapon);

        if (weapon == currentWeaponLeft)
            currentWeaponLeft = null;
    }


    private void ReturnWeaponToInventory(GameObject weapon)
    {
        if (weapon == null) return;

        WeaponPickup pickup = weapon.GetComponentInChildren<WeaponPickup>();
        WeaponSO weaponData = pickup?.weaponData;

        if (weaponData != null)
        {
            InventoryHolder inventory = GetComponent<InventoryHolder>();
            if (inventory != null && inventory.inventoryData.CanAddItem(weaponData))
            {
                inventory.AddItem(weaponData);
                Debug.Log("Returned weapon to inventory: " + weaponData.name);
            }
            else
            {
                Debug.LogWarning("Can't return weapon to inventory");
            }
        }
        else
        {
            Debug.LogWarning("WeaponColllider or WeaponSO is null.");
        }

        Destroy(weapon);
    }
    #endregion
}
