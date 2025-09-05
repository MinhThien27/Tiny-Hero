using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    [SerializeField] private ItemDatabase itemDatabase;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }

    private string saveFilePath => Application.persistentDataPath + "/gameSave.json";

    public void SaveGame()
    {
        var player = FindObjectOfType<PlayerController>();
        var weaponManager = FindObjectOfType<WeaponManager>();
        var inventoryHolder = FindObjectOfType<InventoryHolder>();

        if (player == null || weaponManager == null || inventoryHolder == null)
        {
            Debug.LogWarning("Save failed: missing references!");
            return;
        }

        GameSaveData saveData = new GameSaveData();
        // Scene
        saveData.sceneName = SceneManager.GetActiveScene().name;

        // Player
        saveData.playerHP = player.GetComponent<PlayerHealth>().CurrentHealth;
        saveData.playerGold = GameManager.Instance.Score;

        // Weapons
        saveData.leftHandWeaponID = weaponManager.currentWeaponLeft?.GetComponentInChildren<WeaponCollider>().weaponData.id ?? "";
        saveData.rightHandWeaponID = weaponManager.currentWeaponRight?.GetComponentInChildren<WeaponCollider>().weaponData.id ?? "";

        // Inventory
        foreach (var item in inventoryHolder.inventoryData.items)
        {
            saveData.inventory.Add(new InventoryItemSaveData
            {
                itemID = item.itemData.id,
                quantity = item.quantity
            });
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("Game Saved: " + saveFilePath);
    }

    private WeaponSO GetWeaponDataByID(string id)
    {
        return itemDatabase.GetItemByID(id) as WeaponSO;
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("No save file found!");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

        // Tìm reference mới trong scene sau khi load
        var player = FindObjectOfType<PlayerController>();
        var weaponManager = FindObjectOfType<WeaponManager>();
        var inventoryHolder = FindObjectOfType<InventoryHolder>();

        if (player == null || weaponManager == null || inventoryHolder == null)
        {
            Debug.LogError("Load failed: player, weaponManager, or inventoryHolder not found!");
            return;
        }

        // Load Player
        player.GetComponent<PlayerHealth>().SetHealth(saveData.playerHP, false);
        GameManager.Instance.SetScore(saveData.playerGold);

        // Load Weapons
        Debug.Log("Loading Weapons: LeftHandID=" + saveData.leftHandWeaponID + ", RightHandID=" + saveData.rightHandWeaponID);
        if (!string.IsNullOrEmpty(saveData.leftHandWeaponID))
        { 
            GameObject weaponToEquip = weaponManager.EquipLeftHand(GetWeaponDataByID(saveData.leftHandWeaponID));
            player.EquipWeapon(weaponToEquip.GetComponentInChildren<WeaponCollider>());
        }

        if (!string.IsNullOrEmpty(saveData.rightHandWeaponID))
        {
            GameObject weaponToEquip = weaponManager.EquipRightHand(GetWeaponDataByID(saveData.rightHandWeaponID));
            player.EquipWeapon(weaponToEquip.GetComponentInChildren<WeaponCollider>());
        }

            // Load Inventory
            inventoryHolder.inventoryData.Clear();
        foreach (var item in saveData.inventory)
        {
            BaseItemSO data = itemDatabase.GetItemByID(item.itemID);
            if (data != null)
                inventoryHolder.inventoryData.Add(data, item.quantity);
        }

        Debug.Log("Game Loaded");
    }


    public void ClearSaveData()
    {
        if (File.Exists(saveFilePath))
            File.Delete(saveFilePath);

        Debug.Log("Save data cleared!");
    }

    public string GetSavedSceneName()
    {
        if (!File.Exists(saveFilePath)) return null;

        string json = File.ReadAllText(saveFilePath);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
        return saveData.sceneName;
    }
    public void ContinueGame()
    {
        if (!HasSaveFile())
        {
            Debug.LogWarning("No save file found!");
            return;
        }

        string savedScene = GetSavedSceneName();
        if (string.IsNullOrEmpty(savedScene))
        {
            Debug.LogWarning("No scene name in save file!");
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(savedScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadGame();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
