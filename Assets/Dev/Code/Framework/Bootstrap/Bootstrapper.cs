using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Linq;
using System;

public class Bootstrapper
{
    private static string bootstrapPath = "Assets/Dev/Bootstrap/";
    private static List<BootstrapObjectData> _bootstrapObjectData = new();
    private static Dictionary<string, DirectoryObjectData> _directoryObjectDataByFolderPaths = new Dictionary<string, DirectoryObjectData>();
    private static List<Transform> _spawnedTransformBuffer = new List<Transform>();
    public static Action OnCompleted;

    [RuntimeInitializeOnLoadMethod]
    static void Bootstrap()
    {
        Addressables.InitializeAsync().WaitForCompletion();
        GenerateBootstrapObjectData();
        GenerateDirectoryObjectData();
        SortObjectsByDependencies();
        InstantiatePrefabs();
        ReorderSpawnedTransforms();
        OnCompleted?.Invoke();
    }

    static void GenerateBootstrapObjectData()
    {
        _bootstrapObjectData.Clear();
        var assetsWithLocations = AddressableUtility.LoadAssetsWithLocationsAtPath<GameObject>(bootstrapPath);

        foreach (var asset in assetsWithLocations)
        {
            var bootstrapObjectData = new BootstrapObjectData();
            var hierarchyPath = asset.Location.ToString();
            hierarchyPath = hierarchyPath.Substring(bootstrapPath.Length);
            hierarchyPath = hierarchyPath.Substring(0, hierarchyPath.LastIndexOf('/'));
            bootstrapObjectData.PrefabWithLocation = asset;
            bootstrapObjectData.HierarchyPath = hierarchyPath;
            bootstrapObjectData.MainScriptType = bootstrapObjectData.PrefabWithLocation.Asset.GetComponent<MonoBehaviour>()?.GetType();
            GatherOrderAttributes(bootstrapObjectData);
            _bootstrapObjectData.Add(bootstrapObjectData);
        }
    }

    static void GenerateDirectoryObjectData()
    {
        _directoryObjectDataByFolderPaths.Clear();

        foreach (var data in _bootstrapObjectData)
        {
            string[] folders = data.HierarchyPath.Split('/');
            string currentPath = "";
            DirectoryObjectData parentData = null;

            foreach (var folder in folders)
            {
                currentPath = string.IsNullOrEmpty(currentPath) ? folder : $"{currentPath}/{folder}";

                if (!_directoryObjectDataByFolderPaths.ContainsKey(currentPath))
                {
                    var folderObject = new GameObject(folder);
                    var folderObjectTransform = folderObject.transform;
                    folderObjectTransform.parent = parentData?.Transform;

                    var directoryData = new DirectoryObjectData
                    {
                        HierarchyPath = currentPath,
                        Object = folderObject,
                        Transform = folderObjectTransform,
                        ParentDirectoryObjectData = parentData
                    };
                    _directoryObjectDataByFolderPaths[currentPath] = directoryData;
                }

                parentData = _directoryObjectDataByFolderPaths[currentPath];
            }
        }
    }

    static void GatherOrderAttributes(BootstrapObjectData bootstrapObjectData)
    {
        if (bootstrapObjectData.MainScriptType != null)
        {
            var orderAttr = (BootstrapOrder)Attribute.GetCustomAttribute(bootstrapObjectData.MainScriptType, typeof(BootstrapOrder));
            bootstrapObjectData.Order = orderAttr?.Order ?? 0;

            var beforeAttrs = bootstrapObjectData.MainScriptType.GetCustomAttributes(typeof(BootstrapBefore), true).Cast<BootstrapBefore>();
            var afterAttrs = bootstrapObjectData.MainScriptType.GetCustomAttributes(typeof(BootstrapAfter), true).Cast<BootstrapAfter>();

            foreach (var attr in beforeAttrs)
            {
                bootstrapObjectData.BeforeTypes.Add(attr.Type);
            }

            foreach (var attr in afterAttrs)
            {
                bootstrapObjectData.AfterTypes.Add(attr.Type);
            }
        }
    }

    static void SortObjectsByDependencies()
    {
        _bootstrapObjectData = _bootstrapObjectData
            .OrderBy(data => data.Order)
            .ThenBy(data => data, new DependencyComparer(_bootstrapObjectData))
            .ToList();
    }

    static void InstantiatePrefabs()
    {
        _spawnedTransformBuffer.Clear();

        foreach (var data in _bootstrapObjectData)
        {
            // Logger.LogDebug($"Instantiating: {data.PrefabWithLocation.Asset.name}");
            var instantiatedPrefab = GameObject.Instantiate(data.PrefabWithLocation.Asset, null);
            instantiatedPrefab.name = data.PrefabWithLocation.Asset.name;
            data.Object = instantiatedPrefab;
            data.Transform = instantiatedPrefab.transform;
            _spawnedTransformBuffer.Add(data.Transform);

            if (_directoryObjectDataByFolderPaths.TryGetValue(data.HierarchyPath, out var directoryData))
            {
                data.Transform.parent = directoryData.Transform;
            }
            else
            {
                data.Transform.parent = null;
            }
        }
    }

    static void ReorderSpawnedTransforms()
    {
        var directoryGroups = _spawnedTransformBuffer.GroupBy(t => t.parent);

        foreach (var group in directoryGroups)
        {
            var sortedTransforms = group.OrderBy(t => t.name).ToList();
            for (int i = 0; i < sortedTransforms.Count; i++)
            {
                sortedTransforms[i].SetSiblingIndex(i);
            }
        }
    }

    public class BootstrapObjectData
    {
        public GameObject Object;
        public Type MainScriptType;
        public Transform Transform;
        public AssetWithLocation<GameObject> PrefabWithLocation;
        public string HierarchyPath;
        public int Order;
        public HashSet<Type> BeforeTypes = new HashSet<Type>();
        public HashSet<Type> AfterTypes = new HashSet<Type>();
    }

    public class DirectoryObjectData
    {
        public GameObject Object;
        public Transform Transform;
        public string HierarchyPath;
        public DirectoryObjectData ParentDirectoryObjectData;
    }

    private class DependencyComparer : IComparer<BootstrapObjectData>
    {
        private readonly List<BootstrapObjectData> _bootstrapObjectData;
        private readonly Dictionary<Type, List<Type>> _dependencyGraph;
        private readonly HashSet<Type> _visited;
        private readonly HashSet<Type> _stack;
        private readonly List<Type> _sortedTypes;

        public DependencyComparer(List<BootstrapObjectData> bootstrapObjectData)
        {
            _bootstrapObjectData = bootstrapObjectData;
            _dependencyGraph = new Dictionary<Type, List<Type>>();
            _visited = new HashSet<Type>();
            _stack = new HashSet<Type>();
            _sortedTypes = new List<Type>();

            BuildDependencyGraph();
            TopologicalSort();
        }

        public int Compare(BootstrapObjectData a, BootstrapObjectData b)
        {
            if (a.MainScriptType == null && b.MainScriptType == null) return 0;
            if (a.MainScriptType == null) return -1;
            if (b.MainScriptType == null) return 1;

            if (a.MainScriptType == b.MainScriptType) return 0;

            int aIndex = _sortedTypes.IndexOf(a.MainScriptType);
            int bIndex = _sortedTypes.IndexOf(b.MainScriptType);

            if (aIndex < bIndex) return -1;
            if (aIndex > bIndex) return 1;

            return 0;
        }

        private void BuildDependencyGraph()
        {
            foreach (var data in _bootstrapObjectData)
            {
                var type = data.MainScriptType;
                if (type == null) continue;

                if (!_dependencyGraph.ContainsKey(type))
                    _dependencyGraph[type] = new List<Type>();

                foreach (var beforeType in data.BeforeTypes)
                {
                    if (!_dependencyGraph.ContainsKey(beforeType))
                        _dependencyGraph[beforeType] = new List<Type>();

                    _dependencyGraph[beforeType].Add(type);
                }

                foreach (var afterType in data.AfterTypes)
                {
                    if (!_dependencyGraph.ContainsKey(type))
                        _dependencyGraph[type] = new List<Type>();

                    _dependencyGraph[type].Add(afterType);
                }
            }
        }

        private void TopologicalSort()
        {
            foreach (var type in _dependencyGraph.Keys)
            {
                if (!_visited.Contains(type))
                    if (!DepthFirstSearch(type))
                    {
                        Debug.LogError("Circular dependency detected.");
                        return;
                    }
            }
        }

        private bool DepthFirstSearch(Type type)
        {
            _visited.Add(type);
            _stack.Add(type);

            if (!_dependencyGraph.TryGetValue(type, out var dependencies))
            {
                // If no dependencies, add to sorted list and return true
                _sortedTypes.Add(type);
                return true;
            }

            foreach (var dependentType in dependencies)
            {
                if (!_dependencyGraph.ContainsKey(dependentType))
                {
                    Logger.LogError($"Type {dependentType} specified in dependency attributes is not present in the graph.");
                    continue;
                }

                if (!_visited.Contains(dependentType))
                {
                    if (!DepthFirstSearch(dependentType))
                        return false;
                }
                else if (_stack.Contains(dependentType))
                {
                    return false; // Circular dependency detected
                }
            }

            _stack.Remove(type);
            _sortedTypes.Add(type);

            return true;
        }
    }
}


[AttributeUsage(AttributeTargets.Class)]
public class BootstrapBefore : Attribute
{
    public Type Type { get; private set; }

    public BootstrapBefore(Type type)
    {
        Type = type;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class BootstrapAfter : Attribute
{
    public Type Type { get; private set; }

    public BootstrapAfter(Type type)
    {
        Type = type;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class BootstrapOrder : Attribute
{
    public int Order { get; private set; }

    public BootstrapOrder(int order)
    {
        Order = order;
    }
}

