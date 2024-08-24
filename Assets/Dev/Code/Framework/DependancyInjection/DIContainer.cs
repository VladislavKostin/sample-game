using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Framework
{
    public class DIContainer
    {
        private static readonly Dictionary<Type, object> _injectableObjectsByType = new();
        private static readonly Dictionary<Type, FieldInfo[]> _injectableFieldsByType = new();
        public static bool IsInitialized { get; private set; }
        public static event Action OnInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            // Logger.Log($"Initializing injection.");
            _injectableObjectsByType.Clear();
            _injectableFieldsByType.Clear();
            Bootstrapper.OnCompleted -= ProcessScene;
            Bootstrapper.OnCompleted += ProcessScene;
        }

        public static void ProcessScene()
        {
            var allMonoBehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();
            InjectComponents(allMonoBehaviours);
            InjectFields(allMonoBehaviours);
            IsInitialized = true;
            OnInitialized?.Invoke();
            OnInitialized = null;
        }

        private static void InjectComponents(MonoBehaviour[] allMonoBehaviours)
        {
            foreach (var component in allMonoBehaviours)
            {
                ProcessInjectableObject(component);
            }
        }

        private static void InjectFields(MonoBehaviour[] allMonoBehaviours)
        {
            foreach (var component in allMonoBehaviours)
            {
                ProcessInjectableFields(component);
            }
        }

        private static List<FieldInfo> __injectableFieldsBuffer = new(8);
        public static bool ProcessInjectableFields(object target)
        {
            var targetType = target.GetType();
            var typeProcessed = _injectableFieldsByType.TryGetValue(targetType, out var injectableFields);

            if (!typeProcessed)
            {
                __injectableFieldsBuffer.Clear();
                var fields = ReflectionUtility.GetFields(targetType);
                foreach (var field in fields)
                {
                    var attributes = field.GetCustomAttributes(typeof(InjectAttribute), inherit: true); // TODO: Stop GARBGE production?
                    // var attribute = field.GetAttribute<InjectAttribute>();
                    if (attributes.Length > 0)
                    {
                        __injectableFieldsBuffer.Add(field);
                    }
                }
                injectableFields = __injectableFieldsBuffer.ToArray();
                _injectableFieldsByType[targetType] = injectableFields;
            }

            if (injectableFields == null)
            {
                // Logger.Log($"Object of type #{targetType} does not have any injectable fields.");
                return false;
            }
            else
            {
                foreach (var field in injectableFields)
                {
                    var fieldType = field.FieldType;
                    _injectableObjectsByType.TryGetValue(fieldType, out var injectableObject);

                    if (injectableObject != null)
                    {
                        field.SetValue(target, injectableObject);
                        // Logger.Log($"Object of type #{fieldType} is injected ito a field of #{targetType}.");
                    }
                    else
                    {
                        Logger.LogError($"Injectable object of type #{fieldType} requested by #{targetType} is missing");
                    }
                }
                return true;
            }
        }

        public static void ProcessInjectableObject(object target)
        {
            var targetType = target.GetType();
            var typeProcessed = _injectableObjectsByType.TryGetValue(targetType, out var injectableObject);

            // Logger.Log($"Type #{targetType} processed #{typeProcessed}.");
            if (!typeProcessed)
            {
                if (Attribute.IsDefined(targetType, typeof(InjectAttribute)))
                {
                    injectableObject = target;
                    // Logger.Log($"Object of type #{targetType} is injected as single.");
                }
                else
                {
                    injectableObject = null;
                    // Logger.Log($"Object of type #{targetType} is not injected.");
                }
                _injectableObjectsByType[targetType] = injectableObject;
            }

            if (injectableObject != null)
            {
                if (injectableObject == target)
                {
                    // Logger.Log($"This object of type #{targetType} is already injected");
                }
                else
                {
                    // Logger.LogError($"Object of type #{targetType} is already injected");
                }
            }
        }

        public static void ProcessAssignFields(object target)
        {

        }
    }
}