using System.IO;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
public class Setup
{
    [MenuItem("Tools/Setup/Create Default Folders")]
    public static void CreateDefaultFolders()
    {
        Folders.CreateFolder("_Project", "Scripts", "Animations", "Prefabs", "Scenes", "Materials", "Arts", "Audio", "Textures", "ScriptableObjects", "Settings");
        UnityEditor.AssetDatabase.Refresh();
    }

    public class Folders
    {
        public static void CreateFolder(string root, params string[] folders)
        {
            var fullPath = Path.Combine(Application.dataPath, root);
            foreach (var folder in folders)
            {
                var path = Path.Combine(fullPath, folder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }
    }

    [MenuItem("Tools/Setup/Import Basic Assets")]
    public static void ImportBasicAssets()
    {
        //Assets.ImportAsset("Odin Inspector and Serializer.unitypackage", "Sirenix/Editor ExtensionsSystem");
        //Assets.ImportAsset("Odin Validator.unitypackage", "Sirenix/Editor ExtensionsUtilities");
        //Assets.ImportAsset("Editor Console Pro.unitypackage", "FlyingWorm/Editor ExtensionsSystem");
        //Assets.ImportAsset("DOTween HOTween v2.unitypackage", "Demigiant/ScriptingAnimation");
        //Assets.ImportAsset("Color Studio.unitypackage", "Kronnect/Editor ExtensionsPainting");
        //Assets.ImportAsset("Fullscreen Editor.unitypackage", "Muka Schultze/Editor ExtensionsUtilities");
        //Assets.ImportAsset("Replace Selected.unitypackage", "Staggart Creations/Editor ExtensionsUtilities");
        //Assets.ImportAsset("Selection History.unitypackage", "Staggart Creations/Editor ExtensionsUtilities");
    }

    public static class Assets
    {
        public static void ImportAsset(string asset, string subfolder, string folders = "C:/Users/minht/AppData/Roaming/Unity/Asset Store-5.x")
        {
            AssetDatabase.ImportPackage(Path.Combine(folders, subfolder, asset), false);
        }
    }
}
