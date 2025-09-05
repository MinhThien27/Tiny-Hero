using UnityEngine;
using UnityEngine.UI;

public class CurrentWeaponUI : MonoBehaviour
{
    [Header("References")]
    public WeaponManager weaponManager;
    public InventorySlot leftHandSlotUI;
    public InventorySlot rightHandSlotUI;

    private void OnEnable()
    {
        UpdateUI();
    }

    private void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (weaponManager == null) return;

        leftHandSlotUI.ClearSlot();
        rightHandSlotUI.ClearSlot();

        // Left hand
        if (weaponManager.currentWeaponLeft != null && weaponManager.currentWeaponLeft.GetComponentInChildren<WeaponCollider>().weaponData != null)
        {
            InventoryItem leftItem = new InventoryItem(weaponManager.currentWeaponLeft.GetComponentInChildren<WeaponCollider>().weaponData, 1);
            leftHandSlotUI.Setup(leftItem, null);
        }

        // Right hand
        if (weaponManager.currentWeaponRight != null && weaponManager.currentWeaponRight.GetComponentInChildren<WeaponCollider>().weaponData != null)
        {
            InventoryItem rightItem = new InventoryItem(weaponManager.currentWeaponRight.GetComponentInChildren<WeaponCollider>().weaponData, 1);
            rightHandSlotUI.Setup(rightItem, null);
        }

    }
}
