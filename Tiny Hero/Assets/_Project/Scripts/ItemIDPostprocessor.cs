#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
#if UNITY_EDITOR
public class ItemIDPostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            if (assetPath.EndsWith(".asset"))
            {
                ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);

                // Check if the asset is of type BaseItemSO
                if (asset is BaseItemSO item)
                {
                    if (string.IsNullOrEmpty(item.id))
                    {
                        item.id = Guid.NewGuid().ToString();
                        EditorUtility.SetDirty(item); // Mark the asset as dirty to ensure the change is saved
                        Debug.Log($"Auto-generated ID for: {item.name}");
                    }
                }
            }
        }
    }
}
#endif

