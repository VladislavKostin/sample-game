// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using System.Collections.Generic;
// using System.Linq;
// using System;

// public class Bootstrap
// {
//     private static string bootstrapPath = "Assets/Dev/Bootstrap/";
//     private static List<BootstrapObjectData> _bootstrapObjectData = new();
//     private static Dictionary<string, DirectoryObjectData> _directoryObjectDataByFolderPaths = new Dictionary<string, DirectoryObjectData>();
//     private static List<Transform> _spawnedTransformBuffer = new List<Transform>();
//     public static Action OnCompleted;

//     public class BootstrapObjectData
//     {
//         public GameObject Object;
//         public MonoBehaviour MainScript;
//         public List<Type> MainScriptTypes;
//         public Transform Transform;
//         public AssetWithLocation<GameObject> PrefabWithLocation;
//         public string HierarchyPath;
//         public HashSet<Type> BeforeTypes = new HashSet<Type>();
//         public HashSet<Type> AfterTypes = new HashSet<Type>();
//     }

//     public class DirectoryObjectData
//     {
//         public GameObject Object;
//         public Transform Transform;
//         public string HierarchyPath;
//         public DirectoryObjectData ParentDirectoryObjectData;
//     }

//     [RuntimeInitializeOnLoadMethod]
//     static void OnRuntimeMethodLoad()
//     {
//         Addressables.InitializeAsync().WaitForCompletion();
//         Run();
//         OnCompleted?.Invoke();
//     } 

//     static void Run()
//     {
//         GenerateBootstrapObjectData();
//         GenerateDirectoryObjectData();
//         SortObjectsByDependencies3();
//         InstantiatePrefabs();
//         ReorderSpawnedTransforms();
//     }

//     static void GenerateBootstrapObjectData()
//     {
//         _bootstrapObjectData.Clear();
//         var assetsWithLocations = AddressableUtility.LoadAssetsWithLocationsAtPath<GameObject>(bootstrapPath);

//         foreach (var asset in assetsWithLocations)
//         {
//             var bootstrapObjectData = new BootstrapObjectData();
//             string hierarchyPath = asset.Location.ToString();
//             hierarchyPath = hierarchyPath.Substring(bootstrapPath.Length);
//             hierarchyPath = hierarchyPath.Substring(0, hierarchyPath.LastIndexOf('/'));
//             bootstrapObjectData.PrefabWithLocation = asset;
//             bootstrapObjectData.HierarchyPath = hierarchyPath;
//             CollectMainScriptTypes(bootstrapObjectData);
//             PopulateDependencyAttributes(bootstrapObjectData);
//             _bootstrapObjectData.Add(bootstrapObjectData);
//         }
//     }

//     static void CollectMainScriptTypes(BootstrapObjectData bootstrapObjectData)
//     {
//         bootstrapObjectData.MainScript = bootstrapObjectData.PrefabWithLocation.Asset.GetComponent<MonoBehaviour>();
//         bootstrapObjectData.MainScriptTypes = new List<Type>();
//         if (bootstrapObjectData.MainScript != null)
//         {
//             // Logger.LogDebug($"{bootstrapObjectData.PrefabWithLocation.Asset.name}");

//             var currentType = bootstrapObjectData.MainScript.GetType();

//             while (currentType != null && currentType != typeof(MonoBehaviour))
//             {
//                 bootstrapObjectData.MainScriptTypes.Add(currentType);
//                 currentType = currentType.BaseType;
//                 // Logger.LogDebug($"{currentType}");
//             }
//         }
//     }

//     static void PopulateDependencyAttributes(BootstrapObjectData bootstrapObjectData)
//     {
//         var maintype = bootstrapObjectData.MainScriptTypes.Count > 0 ? bootstrapObjectData.MainScriptTypes[0] : null;
//         if (maintype != null)
//         {
//             var beforeAttrs = maintype.GetCustomAttributes(typeof(BootstrapBefore), true).Cast<BootstrapBefore>();
//             var afterAttrs = maintype.GetCustomAttributes(typeof(BootstrapAfter), true).Cast<BootstrapAfter>();

//             foreach (var attr in beforeAttrs)
//             {
//                 bootstrapObjectData.BeforeTypes.Add(attr.Type);
//             }

//             foreach (var attr in afterAttrs)
//             {
//                 bootstrapObjectData.AfterTypes.Add(attr.Type);
//             }

//             // if (bootstrapObjectData.BeforeTypes.Count != 0 || bootstrapObjectData.AfterTypes.Count != 0)
//             // {
//             //     Logger.LogDebug($"{maintype} before: {bootstrapObjectData.BeforeTypes.Count()}, after: {bootstrapObjectData.AfterTypes.Count()}");
//             // }
//         }
//     }

//     static void SortObjectsByDependencies3()
//     {
//         // Get all unique pairs of objects
//         var comparisons = new List<(BootstrapObjectData a, BootstrapObjectData b, int comparisonResult)>();

//         for (int i = 0; i < _bootstrapObjectData.Count; i++)
//         {
//             for (int j = i + 1; j < _bootstrapObjectData.Count; j++)
//             {
//                 var a = _bootstrapObjectData[i];
//                 var b = _bootstrapObjectData[j];
//                 int result = CompareObjects(a, b);
//                 comparisons.Add((a, b, result));
//             }
//         }

//         // Create a map of objects to their sort index
//         var sortIndex = new Dictionary<BootstrapObjectData, int>();

//         // Initialize the sort index with zero values
//         foreach (var obj in _bootstrapObjectData)
//         {
//             sortIndex[obj] = 0;
//         }

//         // Apply comparisons to adjust sort index
//         foreach (var (a, b, result) in comparisons)
//         {
//             if (result == -1)
//             {
//                 // If a should come before b
//                 sortIndex[a] = Math.Max(sortIndex[a], sortIndex[b] - 1);
//             }
//             else if (result == 1)
//             {
//                 // If b should come before a
//                 sortIndex[b] = Math.Max(sortIndex[b], sortIndex[a] - 1);
//             }
//         }

//         // Sort based on the computed sort index
//         _bootstrapObjectData = _bootstrapObjectData.OrderBy(obj => sortIndex[obj]).ToList();
//     }

//     static int CompareObjects(BootstrapObjectData a, BootstrapObjectData b)
//     {
//         // Logger.LogDebug($"CompareObjects: {a.PrefabWithLocation.Asset.name} vs {b.PrefabWithLocation.Asset.name}");
//         var aBeforeDepth = int.MaxValue;
//         var aAfterDepth = int.MaxValue;
//         for (int typeIndex = 0; typeIndex < b.MainScriptTypes.Count; typeIndex++)
//         {
//             var type = b.MainScriptTypes[typeIndex];
//             if (a.BeforeTypes.Contains(type))
//             {
//                 aBeforeDepth = typeIndex;
//             }
//             if (a.AfterTypes.Contains(type))
//             {
//                 aAfterDepth = typeIndex;
//             }
//         }

//         var bBeforeDepth = int.MaxValue;
//         var bAfterDepth = int.MaxValue;
//         for (int typeIndex = 0; typeIndex < a.MainScriptTypes.Count; typeIndex++)
//         {
//             var type = a.MainScriptTypes[typeIndex];
//             if (b.BeforeTypes.Contains(type))
//             {
//                 bBeforeDepth = typeIndex;
//             }
//             if (b.AfterTypes.Contains(type))
//             {
//                 bAfterDepth = typeIndex;
//             }
//         }

//         // Logger.LogDebug($"aBefore: {aBeforeDepth}, aAfter: {aAfterDepth}, bBefore: {bBeforeDepth}, bAfter: {bAfterDepth}");

//         var minBefore = Math.Min(aBeforeDepth, bAfterDepth);
//         var minAfter = Math.Min(aAfterDepth, bBeforeDepth);

//         if (minBefore < minAfter)
//         {
//             // Logger.LogDebug($"result: -1");
//             return -1;
//         }
//         else if (minBefore > minAfter)
//         {
//             // Logger.LogDebug($"result: 1");
//             return 1;
//         }
//         else
//         {
//             return 0;
//         }
//     }

//     static void GenerateDirectoryObjectData()
//     {
//         _directoryObjectDataByFolderPaths.Clear();

//         foreach (var data in _bootstrapObjectData)
//         {
//             string[] folders = data.HierarchyPath.Split('/');
//             string currentPath = "";
//             DirectoryObjectData parentData = null;

//             foreach (var folder in folders)
//             {
//                 currentPath = string.IsNullOrEmpty(currentPath) ? folder : $"{currentPath}/{folder}";

//                 if (!_directoryObjectDataByFolderPaths.ContainsKey(currentPath))
//                 {
//                     GameObject folderObject = new GameObject(folder);
//                     var folderObjectTransform = folderObject.transform;
//                     folderObjectTransform.parent = parentData?.Transform;

//                     DirectoryObjectData directoryData = new DirectoryObjectData
//                     {
//                         HierarchyPath = currentPath,
//                         Object = folderObject,
//                         Transform = folderObjectTransform,
//                         ParentDirectoryObjectData = parentData
//                     };
//                     _directoryObjectDataByFolderPaths[currentPath] = directoryData;
//                 }

//                 parentData = _directoryObjectDataByFolderPaths[currentPath];
//             }
//         }
//     }

//     static void InstantiatePrefabs()
//     {
//         _spawnedTransformBuffer.Clear();

//         foreach (var data in _bootstrapObjectData)
//         {
//             Logger.LogDebug($"Instantiating: {data.PrefabWithLocation.Asset.name}");

//             GameObject instantiatedPrefab = GameObject.Instantiate(data.PrefabWithLocation.Asset, null);
//             instantiatedPrefab.name = data.PrefabWithLocation.Asset.name;
//             data.Object = instantiatedPrefab;
//             data.Transform = instantiatedPrefab.transform;
//             _spawnedTransformBuffer.Add(data.Transform);

//             if (_directoryObjectDataByFolderPaths.TryGetValue(data.HierarchyPath, out var directoryData))
//             {
//                 data.Transform.parent = directoryData.Transform;
//             }
//             else
//             {
//                 data.Transform.parent = null;
//             }
//         }
//     }

//     static void ReorderSpawnedTransforms()
//     {
//         var directoryGroups = _spawnedTransformBuffer.GroupBy(t => t.parent);

//         foreach (var group in directoryGroups)
//         {
//             var sortedTransforms = group.OrderBy(t => t.name).ToList();
//             for (int i = 0; i < sortedTransforms.Count; i++)
//             {
//                 sortedTransforms[i].SetSiblingIndex(i);
//             }
//         }
//     }
// }

// [AttributeUsage(AttributeTargets.Class)]
// public class BootstrapBefore : Attribute
// {
//     public Type Type { get; private set; }

//     public BootstrapBefore(Type type)
//     {
//         Type = type;
//     }
// }

// [AttributeUsage(AttributeTargets.Class)]
// public class BootstrapAfter : Attribute
// {
//     public Type Type { get; private set; }

//     public BootstrapAfter(Type type)
//     {
//         Type = type;
//     }
// }
