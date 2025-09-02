using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Inventory Config")]
    [SerializeField] private ItemDatabase itemDatabase;
    public InventoryHolder inventoryHolder;
    public string inventoryTitle = "Inventory";
    public KeyCode toggleKey = KeyCode.I;

    [Header("UI References")]
    [SerializeField] private Transform mainPanel;
    [SerializeField] private Transform slotHolder;
    [SerializeField] private GameObject slotOptionsMenu;
    [SerializeField] private GameObject itemInfoPanel;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private Text inventoryTitleText;

    private InventorySlot[] slots;
    private bool isUIOpen = false;
    private InventorySlot currentSelectedSlot;
    private bool isUIInitialized = false;
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/inventorySave.json";
        InitializeInventoryUI();

        LoadInventory();
        Redraw();
    }

    private void OnEnable()
    {
        if (inventoryHolder != null && inventoryHolder.inventoryData != null)
        {
            inventoryHolder.inventoryData.OnInventoryChanged += OnInventoryChangedHandler;
        }
    }

    private void OnDisable()
    {
        if (inventoryHolder != null && inventoryHolder.inventoryData != null)
        {
            inventoryHolder.inventoryData.OnInventoryChanged -= OnInventoryChangedHandler;
        }
    }

    private void OnInventoryChangedHandler()
    {
        Redraw();

        StartCoroutine(SaveInventoryWithDelay());
    }

    private IEnumerator SaveInventoryWithDelay()
    {
        yield return new WaitForSeconds(1);
        SaveInventory();
    }

    void Update()
    {
        if(!isUIInitialized) return;

        CheckForUIToggleInput();
    }

    private void InitializeInventoryUI()
    {
        if (inventoryTitleText != null)
            inventoryTitleText.text = inventoryTitle;
        slots = slotHolder.GetComponentsInChildren<InventorySlot>(includeInactive: true);

        foreach (InventorySlot slot in slots)
        {
            slot.ClearSlot();
            Button btn = slot.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnSlotClicked(slot));
        }

        if (closeButton != null)
        {
            Button closeBtnComponent = closeButton.GetComponent<Button>();
            if (closeBtnComponent != null)
            {
                closeBtnComponent.onClick.RemoveAllListeners();
                closeBtnComponent.onClick.AddListener(() => CloseInventoryUI());
            }
        }

        mainPanel.gameObject.SetActive(false);
        slotOptionsMenu.SetActive(false);
        itemInfoPanel.SetActive(false);

        isUIInitialized = true;
    }

    public void Redraw()
    {
        foreach (var slot in slots)
            slot.ClearSlot();

        var items = inventoryHolder.inventoryData.items;

        for (int i = 0; i < items.Count; i++)
        {
            if (i < slots.Length)
                slots[i].Setup(items[i], inventoryHolder);
        }
    }

    protected void CheckForUIToggleInput()
    {
        if (Input.GetKeyDown(toggleKey)) ToggleUI();
    }

    public void OnSlotClicked(InventorySlot slot)
    {
        if (slot.IsEmpty) return;

        CloseContextMenus();
        ShowSlotOptionsFor(slot);
    }

    private void ShowSlotOptionsFor(InventorySlot slot)
    {
        slotOptionsMenu.transform.position = Input.mousePosition;
        slotOptionsMenu.SetActive(true);

        foreach (Transform child in slotOptionsMenu.transform)
        {
            Button btn = child.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();

            var capturedSlot = slot;
            if (btn.name == "Use Button")
                btn.onClick.AddListener(() => UseItem(capturedSlot));
            else if (btn.name == "Remove Button")
                btn.onClick.AddListener(() => RemoveItem(capturedSlot));
            else if (btn.name == "Infor Button")
                btn.onClick.AddListener(() => ShowItemInfoFor(capturedSlot));
        }
    }
    private void CloseContextMenus()
    {
        slotOptionsMenu.SetActive(false);
        itemInfoPanel.SetActive(false);
        currentSelectedSlot = null;
    }

    private void ShowItemInfoFor(InventorySlot slot)
    {
        if (currentSelectedSlot == slot && itemInfoPanel.activeSelf)
        {
            itemInfoPanel.SetActive(false);
            currentSelectedSlot = null;
            return;
        }

        currentSelectedSlot = slot;

        itemInfoPanel.SetActive(false);

        itemInfoPanel.SetActive(true);
        itemInfoPanel.GetComponentInChildren<Text>().text = slot.item.itemData.description;
    }

    private void RemoveItem(InventorySlot slot)
    {
        if (!slot.IsEmpty)
        {
            inventoryHolder.inventoryData.Remove(slot.item.itemData);
            slotOptionsMenu.SetActive(false);
        }
    }
    private void UseItem(InventorySlot slot)
    {
        if (!slot.IsEmpty)
        {
            inventoryHolder.UseItem(slot.item.itemData);
            slotOptionsMenu.SetActive(false);
        }
    }

    private void ToggleUI()
    {
        isUIOpen = !isUIOpen;
        mainPanel.gameObject.SetActive(isUIOpen);
        if (!isUIOpen) CloseContextMenus();
    }

    private void CloseInventoryUI()
    {
        if (isUIOpen)
        {
            ToggleUI();
        }
    }

    public void SaveInventory()
    {
        InventorySaveData saveData = new();

        foreach (var item in inventoryHolder.inventoryData.items)
        {
            saveData.items.Add(new InventoryItemSaveData
            {
                itemID = item.itemData.id,
                quantity = item.quantity
            });
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("Inventory saved.");
    }
    public void LoadInventory()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("No inventory save file found.");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

        inventoryHolder.inventoryData.Clear();

        foreach (var savedItem in saveData.items)
        {
            BaseItemSO itemData = GetItemDataByID(savedItem.itemID);
            if (itemData != null)
            {
                inventoryHolder.inventoryData.Add(itemData, savedItem.quantity);
            }
            else
            {
                Debug.LogWarning("Không tìm thấy Item với ID: " + savedItem.itemID);
            }
        }

        Debug.Log("Inventory loaded.");
    }

    private BaseItemSO GetItemDataByID(string id)
    {
        return itemDatabase.GetItemByID(id);
    }
}

[System.Serializable]
public class InventoryItemSaveData
{
    public string itemID;
    public int quantity;
}

[System.Serializable]
public class InventorySaveData
{
    public List<InventoryItemSaveData> items = new();
}
