using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    [field: SerializeField] public ItemDatabase ItemDatabase { get; private set; }

    private string saveFilePath => Application.persistentDataPath + "/gameSave.json";
    private GameSaveData pendingSaveData;

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

    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData
        {
            sceneName = SceneManager.GetActiveScene().name
        };

        // Save Player
        var playerSaveable = FindObjectOfType<PlayerSaveable>();
        if (playerSaveable != null)
            saveData.player = playerSaveable.CaptureState();

        // Save Inventory
        var invSaveable = FindObjectOfType<InventorySaveable>();
        if (invSaveable != null)
            saveData.inventory = invSaveable.CaptureState();

        // Save Weapons
        var weaponSaveable = FindObjectOfType<WeaponSaveable>();
        if (weaponSaveable != null)
            saveData.weapons = weaponSaveable.CaptureState();

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("Game Saved: " + saveFilePath);
    }

    public void ContinueGame()
    {
        if (!HasSaveFile()) return;

        string json = File.ReadAllText(saveFilePath);
        pendingSaveData = JsonUtility.FromJson<GameSaveData>(json);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(pendingSaveData.sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplySaveData(pendingSaveData);
        pendingSaveData = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ApplySaveData(GameSaveData saveData)
    {
        // Player
        var playerSaveable = FindObjectOfType<PlayerSaveable>();
        if (playerSaveable != null && saveData.player != null)
            playerSaveable.RestoreState(saveData.player);

        // Inventory
        var invSaveable = FindObjectOfType<InventorySaveable>();
        if (invSaveable != null && saveData.inventory != null)
            invSaveable.RestoreState(saveData.inventory);

        // Weapons
        var weaponSaveable = FindObjectOfType<WeaponSaveable>();
        if (weaponSaveable != null && saveData.weapons != null)
            weaponSaveable.RestoreState(saveData.weapons);

        Debug.Log("Game Loaded & Applied");
    }

    public bool HasSaveFile() => File.Exists(saveFilePath);

    public void ClearSaveData()
    {
        if (HasSaveFile())
            File.Delete(saveFilePath);
    }
}
