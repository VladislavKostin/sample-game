// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEditor;
// using UnityEngine;

// namespace Game.Core
// {
//     [Serializable]
//     public class StateCollection
//     {
//         [EnumFlags]
//         public EnumFlagGroup ActiveStates;
//     }

//     [Serializable]
//     public class EnumFlagGroup
//     {
//         public GameState ActiveGameState;
//         public GameplayState ActiveGameplayState;
//         public SimulationState ActiveSimulationState;
//     }

//     [CustomPropertyDrawer(typeof(StateCollection))]
//     public class StateCollectionDrawer : PropertyDrawer
//     {
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             EditorGUI.BeginProperty(position, label, property);

//             EditorGUI.LabelField(position, label);

//             EditorGUI.indentLevel++;

//             SerializedProperty activeStatesProp = property.FindPropertyRelative("ActiveStates");

//             EditorGUILayout.PropertyField(activeStatesProp.FindPropertyRelative("ActiveGameState"), new GUIContent("Game States"));
//             EditorGUILayout.PropertyField(activeStatesProp.FindPropertyRelative("ActiveGameplayState"), new GUIContent("Gameplay States"));
//             EditorGUILayout.PropertyField(activeStatesProp.FindPropertyRelative("ActiveSimulationState"), new GUIContent("Simulation States"));

//             EditorGUI.indentLevel--;

//             EditorGUI.EndProperty();
//         }
//     }

//     [AttributeUsage(AttributeTargets.Field)]
//     public class EnumFlagsAttribute : PropertyAttribute { }

//     // public class StateActivatable : MonoBehaviour
//     // {
//     //     [SerializeField] private StateCollection _activeStates;

//     //     private void Start()
//     //     {
//     //         // Access _activeStates.ActiveStates.ActiveGameState, _activeStates.ActiveStates.ActiveGameplayState, _activeStates.ActiveStates.ActiveSimulationState
//     //         // to get the selected enum values
//     //     }
//     // }
// }