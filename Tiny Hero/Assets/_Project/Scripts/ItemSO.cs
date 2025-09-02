using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InventoryItem
{
    public BaseItemSO itemData;
    public int quantity;

    public InventoryItem(BaseItemSO so, int qty = 1)
    {
        itemData = so;
        quantity = qty;
    }
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Item/ItemSO", order = 0)]
public class ItemSO : BaseItemSO
{
    public ItemType itemType;
    public GameObject effectPrefab;
    public int value = 10;
    public int maxStack = 10;
    public bool ApplyEffect(PlayerController player)
    {
        switch (itemType)
        {
            case ItemType.HealthPotion:
                return ApplyHealth(player);
            case ItemType.ManaPotion:
                return ApplyMana(player);
            default:
                Debug.LogWarning("Unknown item type: " + itemType);
                return false;
        }
    }

    bool ApplyHealth(PlayerController player)
    {
        if (player.health.IsFullHealth()) return false;
        player.health.Heal(value);
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, player.transform.position, Quaternion.identity, player.transform);
            //Destroy(effect, 2f);
        }
        return true;
    }

    bool ApplyMana(PlayerController player)
    {
        if (player.mana.IsFullMana()) return false;
        player.mana.RestoreMana(value);
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, player.transform.position, Quaternion.identity, player.transform);
            //Destroy(effect, 2f);
        }
        return true;
    }

}
public enum ItemType
{
    HealthPotion,
    ManaPotion
}
