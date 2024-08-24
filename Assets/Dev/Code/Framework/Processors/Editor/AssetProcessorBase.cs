using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets;
using System.Collections.Generic;
using System.Linq;

public abstract class AssetProcessorBase : AssetPostprocessor
{
    public static AssetProcessorBase Instance;
    public static string[] _includedDirectories = null;
    public static string[] _excludedDirectories = null;
    public static HashSet<System.Type> _includedTypes = new HashSet<System.Type>();
    public static HashSet<System.Type> _excludedTypes = new HashSet<System.Type>();

    public static System.Action<Object> OnMovedOutAction;
    public static System.Action<Object> OnMovedInAction;
    public static System.Action<Object> OnMovedAction;
    public static System.Action<Object> OnImportedAction;
    public static System.Action<Object> OnMovedInOrImportedAction;
    public static System.Action<Object> OnMovedOrImportedAction;

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (var importedPath in importedAssets)
        {
            if (CheckPathMatch(importedPath))
            {
                var type = AssetDatabase.GetMainAssetTypeAtPath(importedPath);
                if (CheckTypeMatch(type))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<Object>(importedPath);
                    OnImportedAction?.Invoke(asset);
                    OnMovedInOrImportedAction?.Invoke(asset);
                    OnMovedOrImportedAction?.Invoke(asset);
                }
            }
        }

        for (int i = 0; i < movedAssets.Length; i++)
        {
            var newPath = movedAssets[i];
            var oldPath = movedFromAssetPaths[i];
            var type = AssetDatabase.GetMainAssetTypeAtPath(newPath);
            if (CheckTypeMatch(type))
            {
                var asset = AssetDatabase.LoadAssetAtPath<Object>(newPath);
                var oldPathIsInside = CheckPathMatch(oldPath);
                var newPathIsInside = CheckPathMatch(newPath);

                if (!oldPathIsInside && newPathIsInside)
                {
                    OnMovedAction?.Invoke(asset);
                    OnMovedInAction?.Invoke(asset);
                    OnMovedInOrImportedAction?.Invoke(asset);
                    OnMovedOrImportedAction?.Invoke(asset);
                }
                else if (oldPathIsInside && !newPathIsInside)
                {
                    OnMovedAction?.Invoke(asset);
                    OnMovedOutAction?.Invoke(asset);
                    OnMovedOrImportedAction?.Invoke(asset);
                }
                else
                {
                    OnMovedAction?.Invoke(asset);
                    OnMovedOrImportedAction?.Invoke(asset);
                }
            }
        }
    }

    public static void IncludeDirectories(params string[] includedDirectories)
    {
        _includedDirectories = includedDirectories;
    }

    public static void ExcludeDirectories(params string[] excludedDirectories)
    {
        _excludedDirectories = excludedDirectories;
    }

    public static void IncludeType<T>() where T : Object
    {
        _includedTypes.Add(typeof(T));
    }

    public static void IncludeTypes(params System.Type[] types)
    {
        foreach (var type in types)
        {
            _includedTypes.Add(type);
        }
    }

    public static void ExcludeType<T>() where T : Object
    {
        _excludedTypes.Add(typeof(T));
    }

    public static void ExcludeTypes(params System.Type[] types)
    {
        foreach (var type in types)
        {
            _excludedTypes.Add(type);
        }
    }

    public static bool CheckPathMatch(string assetPath)
    {
        if (_excludedDirectories != null)
        {
            foreach (string excludedDirectory in _excludedDirectories)
            {
                if (assetPath.Contains(excludedDirectory))
                {
                    return false;
                }
            }
        }

        if (_includedDirectories != null)
        {
            foreach (string includedDirectory in _includedDirectories)
            {
                if (assetPath.Contains(includedDirectory))
                {
                    return true;
                }
            }
            return false;
        }
        return true;
    }

    public static bool CheckTypeMatch(System.Type assetType)
    {
        if (_excludedTypes.Contains(assetType))
            return false;

        if (_includedTypes.Count > 0)
            return _includedTypes.Contains(assetType);

        return true;
    }

    protected static bool IsAssetAddressable(Object asset)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return false;

        foreach (var entry in settings.groups.SelectMany(group => group.entries))
        {
            if (entry.MainAsset == asset)
            {
                return true;
            }
        }
        return false;
    }

    protected static void AddAssetToAddressables(Object asset)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return;

        var group = settings.DefaultGroup;
        string assetPath = AssetDatabase.GetAssetPath(asset);

        var entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(assetPath), group);
        entry.address = asset.name;

        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
        AssetDatabase.SaveAssets();
    }

    public static void SetOnMovedOut(System.Action<Object> action) => OnMovedOutAction = action;
    public static void SetOnMovedIn(System.Action<Object> action) => OnMovedInAction = action;
    public static void SetOnMoved(System.Action<Object> action) => OnMovedAction = action;
    public static void SetOnImported(System.Action<Object> action) => OnImportedAction = action;
    public static void SetOnMovedInOrImported(System.Action<Object> action) => OnMovedInOrImportedAction = action;
    public static void SetOnMovedOrImported(System.Action<Object> action) => OnMovedOrImportedAction = action;
}

// public class BootstrapPrefabProcessor : MyAssetPostprocessor
// {
//     [InitializeOnLoadMethod]
//     private static void Initialize()
//     {
//         Instance = new BootstrapPrefabProcessor();
//         IncludeDirectories("Dev/Bootstrap/");
//         IncludeType<GameObject>();
//         SetOnMovedOrImported(asset =>
//         {
//             var prefab = asset as GameObject;
//             if (prefab != null && !IsAssetAddressable(prefab))
//             {
//                 AddAssetToAddressables(prefab);
//             }
//         });
//     }
// }
