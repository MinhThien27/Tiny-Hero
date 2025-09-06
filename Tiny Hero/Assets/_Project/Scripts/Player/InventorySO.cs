using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Game/Inventory/Inventory")]
public class InventorySO : ScriptableObject
{
    public int maxSlots = 20;
    public event Action OnInventoryChanged;
    public List<InventoryItem> items = new();

    public bool CanAddItem(BaseItemSO item)
    {
        if (items.Exists(i => i.itemData == item))
            return true;

        return items.Count < maxSlots;
    }

    public void Add(BaseItemSO item)
    {
        var existingItem = items.Find(i => i.itemData == item);
        if (existingItem != null)
        {
            existingItem.quantity++;
        }
        else
        {
            if (items.Count >= maxSlots)
            {
                Debug.LogWarning("Inventory is full!");
                return;
            }

            items.Add(new InventoryItem(item, 1));
        }

        OnInventoryChanged?.Invoke();
    }
    public void Add(BaseItemSO item, int amount)
    {
        var existingItem = items.Find(i => i.itemData == item);
        if (existingItem != null)
        {
            existingItem.quantity += amount;
        }
        else
        {
            if (items.Count >= maxSlots)
            {
                Debug.LogWarning("Inventory is full!");
                return;
            }

            items.Add(new InventoryItem(item, amount));
        }

        OnInventoryChanged?.Invoke();
    }

    public void Remove(BaseItemSO item)
    {
        var existingItem = items.Find(i => i.itemData == item);
        if (existingItem != null)
        {
            existingItem.quantity--;
            if (existingItem.quantity <= 0)
                items.Remove(existingItem);

            OnInventoryChanged?.Invoke();
        }
    }

    public void Clear()
    {
        items.Clear();
        OnInventoryChanged?.Invoke();
    }
}
