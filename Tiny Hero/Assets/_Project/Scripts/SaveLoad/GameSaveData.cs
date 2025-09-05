using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public string sceneName;
    public string leftHandWeaponID;
    public string rightHandWeaponID;
    public int playerHP;
    public int playerGold;
    public List<InventoryItemSaveData> inventory = new();
    public List<EnemySaveData> enemies = new();
}

[System.Serializable]
public class EnemySaveData
{
    public string enemyID;       
    public Vector3 position;    
    public int hp;
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
