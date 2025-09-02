using UnityEngine;

public class InventoryHolder : MonoBehaviour
{
    public InventoryUI inventoryUI;
    public InventorySO inventoryData;

    private void Awake()
    {
        if (inventoryData == null)
        {
            Debug.LogError("Inventory Data is not assigned in InventoryHolder on " + gameObject.name);
        }
        if (inventoryUI == null)
        {
            inventoryUI = FindAnyObjectByType<InventoryUI>();
        }
    }

    public void AddItem(BaseItemSO item)
    {
        if (inventoryData.CanAddItem(item))
        {
            inventoryData.Add(item);
            Debug.Log("Picked up: " + item.name);
        }
        else
        {
            Debug.Log("Inventory full, cannot pick up: " + item.name);
        }
    }

    public void UseItem(BaseItemSO item)
    {
        PlayerController player = GetComponent<PlayerController>();

        if (item is ItemSO consumable)
        {
            if (consumable.ApplyEffect(player))
            {
                inventoryData.Remove(item);
                Debug.Log("Used: " + item.name);
            }
            else
            {
                Debug.Log("Cannot use item: " + item.name);
            }
        }
        else if (item is WeaponSO weapon)
        {
            EquipWeapon(weapon);
            Debug.Log("Equipped weapon: " + item.name);
            inventoryData.Remove(item);
        }
        else
        {
            Debug.LogWarning("Unknown item type: " + item.name);
        }
    }

    private void EquipWeapon(WeaponSO weapon)
    {
        PlayerController player = GetComponent<PlayerController>();
        if (player != null)
        {
            WeaponManager weaponManager = GetComponent<WeaponManager>();
            GameObject weaponToEquip = weaponManager.EquipWeapon(weapon);
            player.PickupWeapon(weaponToEquip.GetComponentInChildren<WeaponCollider>());
        }
        else
        {
            Debug.LogWarning("WeaponManager component not found on player.");
        }
    }
}
