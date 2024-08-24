// using System.Collections.Generic;
// using UnityEngine;
// using Game.Framework;

// namespace Game.Core
// {
//     [DefaultExecutionOrder(-10)]
//     public class SystemInstaller : MonoInstaller
//     {
//         public new void Start()
//         {
//             InstallBindings();
//             Logger.Log("All systems installed");
//         }

//         public override void InstallBindings()
//         {
//             var systems = FindObjectsOfType<ObjectSystem>();

//             foreach (var system in systems)
//             {
//                 Logger.Log($"{system.GetType()} binding by {gameObject} from {system.gameObject}");

//                 Container.Bind(system.GetType()).FromInstance(system).AsSingle();
//                 Logger.Log($"{system.GetType()} is bound");
//             }
//         }
//     }
// }