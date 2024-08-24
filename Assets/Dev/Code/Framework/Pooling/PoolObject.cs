using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Framework;

namespace Game.Core
{
    public class PoolObject : MonoBehaviour
    {
        [Inject] private PoolingSystem _poolingSystem;

        [SerializeField] private List<ISpawnCallbacksReciever> _spawnCallbacksRecievers;

        public event System.Action OnSpawn;
        public event System.Action OnDespawn;
        public bool IsSpawned { get; private set; }
        public GameObject GameObject { get; private set; }

        private void Awake()
        {
            foreach (var item in _spawnCallbacksRecievers)
            {
                OnSpawn += item.OnSpawn;
                OnDespawn += item.OnDespawn;
            }
            GameObject = gameObject;
        }

        public void InvokeOnSpawn()
        {
            OnSpawn?.Invoke();
        }

        public void InvokeOnDespawn()
        {
            OnDespawn?.Invoke();
        }

        public void Despawn()
        {
            _poolingSystem.ReturnObject(this);
        }
    }
}
