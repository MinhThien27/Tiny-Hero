#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.IO;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class BaseItemSO : ScriptableObject
{
    public string id;
    public string itemName;
    public Sprite icon;
    public string description;
    public GameObject itemPrefab;

#if UNITY_EDITOR
    private void OnValidate()
    {
        //If in import mode or play mode, do nothing
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        if (string.IsNullOrEmpty(id))
        {
            id = Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
        }
    }
#endif
}


