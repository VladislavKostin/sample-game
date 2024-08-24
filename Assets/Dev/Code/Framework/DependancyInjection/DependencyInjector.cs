using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Game.Framework
{
    public class DependencyInjector : MonoBehaviour
    {
        private static Dictionary<Type, bool> _isInjectableByType = new();
        private static Dictionary<Type, bool> _hasInjectableFielldsByType = new();

        [HideInInspector][SerializeField] private List<MonoBehaviour> _componentsWithInjectableFields;
        [HideInInspector][SerializeField] private List<MonoBehaviour> _injectableComponents;

        private void Awake()
        {
            if (DIContainer.IsInitialized)
            {
                // Logger.LogDebug($"IITIALIZED!");
                InjectFields();
            }
            else
            {
                // Logger.LogDebug($"NOT IITIALIZED!");
                DIContainer.OnInitialized += InjectFields;
            }
        }

        public void OnValidate()
        {
            CacheComponents();
        }

        private void CacheComponents()
        {
            _injectableComponents.Clear();
            _componentsWithInjectableFields.Clear();
            var allMonoBehaviours = GetComponents<MonoBehaviour>();
            foreach (var target in allMonoBehaviours)
            {
                // Logger.LogDebug($"Caching {target}");
                var targetType = target.GetType();
                var cached = _hasInjectableFielldsByType.ContainsKey(targetType);
                if (!cached)
                {
                    var hasInectableFields = false;
                    var fields = ReflectionUtility.GetFields(targetType);
                    foreach (var field in fields)
                    {
                        if (Attribute.IsDefined(targetType, typeof(InjectAttribute)))
                        {
                            hasInectableFields = true;
                            // Logger.LogDebug($"#{targetType} has injectable fields.");
                            break;
                        }
                    }
                    _hasInjectableFielldsByType[targetType] = hasInectableFields;

                    var isInjectable = false;
                    if (Attribute.IsDefined(targetType, typeof(InjectAttribute)))
                    {
                        isInjectable = true;
                        // Logger.Log($"#{targetType} is injectable.");
                    }
                    _isInjectableByType[targetType] = isInjectable;
                }
                // Logger.Log($"{_isInjectableByType[targetType]}");

                var isCached = _isInjectableByType[targetType] && _hasInjectableFielldsByType[targetType];
                if (_isInjectableByType[targetType])
                {
                    _injectableComponents.Add(target);
                }
                if (_hasInjectableFielldsByType[targetType])
                {
                    _componentsWithInjectableFields.Add(target);
                }
                // Logger.Log($"#{targetType} is cached.");
            }
        }

        private void InjectFields()
        {
            Logger.LogDebug($"InjectFields");

            foreach (var component in _injectableComponents)
            {
                DIContainer.ProcessInjectableFields(component);
            }
        }

        private void InjectComponents()
        {
            foreach (var component in _componentsWithInjectableFields)
            {
                DIContainer.ProcessInjectableObject(component);
            }
        }
    }
}