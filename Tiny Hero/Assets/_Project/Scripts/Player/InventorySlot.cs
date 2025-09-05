using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Elements")]
    public Image icon;
    public TMP_Text itemQuantityText;

    [HideInInspector] public InventoryItem item;
    [HideInInspector] public InventoryHolder inventoryHolder;

    public bool IsEmpty => item == null || item.itemData == null;

    public void Setup(InventoryItem newItem, InventoryHolder holder)
    {
        item = newItem;
        inventoryHolder = holder;

        if (icon != null)
        {
            icon.sprite = item.itemData.icon;
            icon.gameObject.SetActive(true);
        }

        if (itemQuantityText != null)
            itemQuantityText.text = item.quantity > 1 ? item.quantity.ToString() : "";
    }

    public void ClearSlot()
    {
        item = null;

        if (icon == null) return;

        icon.sprite = null;
        icon.gameObject.SetActive(false);

        if (itemQuantityText != null)
            itemQuantityText.text = "";
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsEmpty && inventoryHolder != null && inventoryHolder.inventoryUI != null)
        {
            inventoryHolder.inventoryUI.SelectSlot(this);
        }
    }
}