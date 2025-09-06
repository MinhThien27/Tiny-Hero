using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/Inventory/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<BaseItemSO> allItems;

    public BaseItemSO GetItemByID(string id)
    {
        return allItems.Find(item => item.id == id);
    }
}
