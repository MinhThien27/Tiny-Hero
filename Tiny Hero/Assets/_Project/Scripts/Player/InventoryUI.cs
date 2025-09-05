using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Inventory Config")]
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

    private void Awake()
    {
        InitializeInventoryUI();
        Redraw();
    }

    private void OnEnable()
    {
        if (inventoryHolder != null && inventoryHolder.inventoryData != null)
            inventoryHolder.inventoryData.OnInventoryChanged += OnInventoryChangedHandler;
    }

    private void OnDisable()
    {
        if (inventoryHolder != null && inventoryHolder.inventoryData != null)
            inventoryHolder.inventoryData.OnInventoryChanged -= OnInventoryChangedHandler;
    }

    private void OnInventoryChangedHandler()
    {
        Redraw();
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
            Button closeBtn = closeButton.GetComponent<Button>();
            if (closeBtn != null)
            {
                closeBtn.onClick.RemoveAllListeners();
                closeBtn.onClick.AddListener(() => CloseInventoryUI());
            }
        }

        mainPanel.gameObject.SetActive(false);
        slotOptionsMenu.SetActive(false);
        itemInfoPanel.SetActive(false);

        isUIInitialized = true;
    }

    public void Redraw()
    {
        foreach (var slot in slots) slot.ClearSlot();

        var items = inventoryHolder.inventoryData.items;
        for (int i = 0; i < items.Count; i++)
        {
            if (i < slots.Length)
                slots[i].Setup(items[i], inventoryHolder);
        }
    }

    void Update()
    {
        if (!isUIInitialized) return;
        if (Input.GetKeyDown(toggleKey)) ToggleUI();
    }

    private void OnSlotClicked(InventorySlot slot)
    {
        if (slot.IsEmpty) return;
        ShowSlotOptionsFor(slot);
    }
    public void SelectSlot(InventorySlot slot)
    {
        if (slot.IsEmpty) return;
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

    private void ShowItemInfoFor(InventorySlot slot)
    {
        itemInfoPanel.SetActive(true);
        itemInfoPanel.GetComponentInChildren<Text>().text = slot.item.itemData.description;
    }

    private void RemoveItem(InventorySlot slot)
    {
        inventoryHolder.inventoryData.Remove(slot.item.itemData);
        slotOptionsMenu.SetActive(false);
    }

    private void UseItem(InventorySlot slot)
    {
        inventoryHolder.UseItem(slot.item.itemData);
        slotOptionsMenu.SetActive(false);
    }

    private void ToggleUI()
    {
        isUIOpen = !isUIOpen;
        mainPanel.gameObject.SetActive(isUIOpen);
        if (!isUIOpen)
        {
            slotOptionsMenu.SetActive(false);
            itemInfoPanel.SetActive(false);
        }
    }

    private void CloseInventoryUI()
    {
        if (isUIOpen) ToggleUI();
    }
}
