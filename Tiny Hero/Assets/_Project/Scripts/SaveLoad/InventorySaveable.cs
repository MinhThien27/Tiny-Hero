using UnityEngine;

public class InventorySaveable : MonoBehaviour, ISaveable<InventorySaveData>
{
    public InventorySaveData CaptureState()
    {
        var holder = GetComponent<InventoryHolder>();
        InventorySaveData saveData = new InventorySaveData();

        foreach (var item in holder.inventoryData.items)
        {
            saveData.items.Add(new InventoryItemSaveData
            {
                itemID = item.itemData.id,
                itemName = item.itemData.name,
                quantity = item.quantity
            });
        }

        return saveData;
    }

    public void RestoreState(InventorySaveData data)
    {
        var holder = GetComponent<InventoryHolder>();
        holder.inventoryData.Clear();

        foreach (var item in data.items)
        {
            var itemData = SaveManager.Instance?.ItemDatabase?.GetItemByID(item.itemID);
            if (itemData != null)
            {
                holder.inventoryData.Add(itemData, item.quantity);
            }
            else
            {
                Debug.LogWarning("Item not found in DB: " + item.itemID);
            }
        }
    }

}
