using UnityEngine;
public class ItemPickup : MonoBehaviour
{
    public ItemSO itemData;

    private void OnTriggerEnter(Collider other)
    {
        InventoryHolder inventory = other.GetComponent<InventoryHolder>();
        if (inventory != null)
        {
            if (inventory.inventoryData.CanAddItem(itemData))
            {
                inventory.AddItem(itemData);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Cannot pick up item. Inventory full.");
            }
        }
    }
}