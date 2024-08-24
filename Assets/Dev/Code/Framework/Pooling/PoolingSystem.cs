using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    [BootstrapBefore(typeof(GameControllerSystem))]
    public class PoolingSystem : ManagedSystem
    {
        private Dictionary<int, Queue<PoolObject>> _poolDictionary = new();

        private void Awake()
        {
            // _poolDictionary = new Dictionary<int, Queue<PoolObject>>();
        }

        public void CreatePool(PoolObject prefab, int poolSize)
        {
            int prefabId = prefab.GetInstanceID();
            if (!_poolDictionary.ContainsKey(prefabId))
            {
                _poolDictionary[prefabId] = new Queue<PoolObject>();
                for (int i = 0; i < poolSize; i++)
                {
                    PoolObject obj = Instantiate(prefab);
                    obj.GameObject.SetActive(false);
                    _poolDictionary[prefabId].Enqueue(obj);
                }
            }
        }

        public PoolObject GetObject(PoolObject prefab)
        {
            int prefabId = prefab.GetInstanceID();
            Queue<PoolObject> queue;
            if (!_poolDictionary.TryGetValue(prefabId, out queue))
            {
                queue = new Queue<PoolObject>();
                _poolDictionary[prefabId] = queue;
            }
            PoolObject poolObject = null;
            if (queue.Count > 0)
            {
                poolObject = queue.Dequeue();
                poolObject.GameObject.SetActive(true);
            }
            else
            {
                poolObject = Instantiate(prefab);
                poolObject.GameObject.SetActive(true);
                var objId = poolObject.GetInstanceID();
                _poolDictionary[objId] = queue;
            }
            return poolObject;
        }

        public void ReturnObject(PoolObject poolObject, bool force = false)
        {
            int objId = poolObject.GetInstanceID();
            if (_poolDictionary.TryGetValue(objId, out var queue))
            {
                poolObject.InvokeOnDespawn();
                poolObject.GameObject.SetActive(false);
                queue.Enqueue(poolObject);
            }
        }
    }
}