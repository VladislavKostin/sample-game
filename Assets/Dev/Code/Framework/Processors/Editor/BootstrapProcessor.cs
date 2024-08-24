using UnityEditor;
using UnityEngine;

public class BootstrapPrefabProcessor : AssetProcessorBase
{
    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        Instance = new BootstrapPrefabProcessor();
        IncludeDirectories("Dev/Bootstrap/");
        IncludeType<GameObject>();
        SetOnMovedOrImported(asset =>
        {
            // var prefab = asset as GameObject;
            // if (!IsAssetAddressable(prefab))
            // {
            //     AddAssetToAddressables(prefab);
            // }
            // var bootstrapObject = AddComponentIfNotPresent<BootstrapObject>(prefab);
            //     var path = AssetDatabase.GetAssetPath(prefab);
            //     path.Remove(0, path.IndexOf("Bootstrap/") + 10);
            //     bootstrapObject.Path = path;
            // });
        });
    }
}
