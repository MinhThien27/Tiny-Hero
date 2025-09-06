using UnityEngine;

public class WeaponSaveable : MonoBehaviour, ISaveable<WeaponSaveData>
{
    public WeaponSaveData CaptureState()
    {
        var wm = GetComponent<WeaponManager>();

        return new WeaponSaveData
        {
            leftHandWeapon = wm.currentWeaponLeft ?
                new WeaponItemSaveData 
                { 
                    weaponID = wm.currentWeaponLeft.GetComponentInChildren<WeaponCollider>().weaponData.id,
                    weaponName = wm.currentWeaponLeft.GetComponentInChildren<WeaponCollider>().weaponData.itemName
                }
                : null,

            rightHandWeapon = wm.currentWeaponRight ?
                new WeaponItemSaveData 
                { 
                    weaponID = wm.currentWeaponRight.GetComponentInChildren<WeaponCollider>().weaponData.id,
                    weaponName = wm.currentWeaponRight.GetComponentInChildren<WeaponCollider>().weaponData.itemName
                }
                : null
        };
    }

    public void RestoreState(WeaponSaveData data)
    {
        var wm = GetComponent<WeaponManager>();
        var player = GetComponent<PlayerController>();

        if (data.leftHandWeapon != null)
        {
            var weaponSO = SaveManager.Instance.ItemDatabase.GetItemByID(data.leftHandWeapon.weaponID) as WeaponSO;
            if (weaponSO != null)
            {
                GameObject obj = wm.EquipLeftHand(weaponSO);
                player.EquipWeapon(obj.GetComponentInChildren<WeaponCollider>());
            }
        }

        if (data.rightHandWeapon != null)
        {
            var weaponSO = SaveManager.Instance.ItemDatabase.GetItemByID(data.rightHandWeapon.weaponID) as WeaponSO;
            if (weaponSO != null)
            {
                GameObject obj = wm.EquipRightHand(weaponSO);
                player.EquipWeapon(obj.GetComponentInChildren<WeaponCollider>());
            }
        }
    }
}
