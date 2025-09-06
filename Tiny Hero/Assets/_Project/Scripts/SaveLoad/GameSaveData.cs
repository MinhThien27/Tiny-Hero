using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public string sceneName;
    public PlayerSaveData player;
    public InventorySaveData inventory;
    public WeaponSaveData weapons;
    public List<EnemySaveData> enemies = new();
}

[Serializable]
public class PlayerSaveData
{
    public Vector3 position;
    public int hp;
    public int gold;
}

[Serializable]
public class InventorySaveData
{
    public List<InventoryItemSaveData> items = new();
}

[Serializable]
public class InventoryItemSaveData
{
    public string itemID;
    public string itemName;
    public int quantity;
}

[Serializable]
public class WeaponSaveData
{
    public WeaponItemSaveData leftHandWeapon;
    public WeaponItemSaveData rightHandWeapon;
}

[Serializable]
public class WeaponItemSaveData
{
    public string weaponID;
    public string weaponName;
}

[Serializable]
public class EnemySaveData
{
    public string enemyID;
    public Vector3 position;
    public int hp;
}
