using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Services
{
    public class ObjectPool : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
            public Transform parent;
        }

        [SerializeField] private List<Pool> _pools;
        private Dictionary<string, Queue<GameObject>> _poolDictionary;

        public void InitializePools()
        {
            _poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in _pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = CreateNewObject(pool);
                    objectPool.Enqueue(obj);
                }

                _poolDictionary.Add(pool.tag, objectPool);
            }
        }

        private GameObject CreateNewObject(Pool pool)
        {
            GameObject obj = Instantiate(pool.prefab, pool.parent);
            obj.SetActive(false);
            return obj;
        }


        public GameObject SpawnFromPool(string tag)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                return null;
            }

            Queue<GameObject> poolQueue = _poolDictionary[tag];
            GameObject objectToSpawn = null;


            foreach (var obj in poolQueue)
            {
                if (!obj.activeInHierarchy)
                {
                    objectToSpawn = obj;
                    break;
                }
            }

            if (objectToSpawn == null)
            {
                Pool poolConfig = _pools.Find(p => p.tag == tag);
                if (poolConfig != null)
                {
                    objectToSpawn = CreateNewObject(poolConfig);
                    poolQueue.Enqueue(objectToSpawn);
                }
            }

            if (objectToSpawn != null)
            {
                objectToSpawn.SetActive(true);
                IPoolableObject poolable = objectToSpawn.GetComponent<IPoolableObject>();
                poolable?.OnSpawn();
            }

            return objectToSpawn;
        }


        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!_poolDictionary.ContainsKey(tag))
                return null;

            Queue<GameObject> poolQueue = _poolDictionary[tag];
            GameObject objectToSpawn = null;

            foreach (var obj in poolQueue)
            {
                if (!obj.activeInHierarchy)
                {
                    objectToSpawn = obj;
                    break;
                }
            }

            if (objectToSpawn == null)
            {
                Pool poolConfig = _pools.Find(p => p.tag == tag);
                if (poolConfig != null)
                {
                    objectToSpawn = CreateNewObject(poolConfig);
                    poolQueue.Enqueue(objectToSpawn);
                }
            }

            if (objectToSpawn != null)
            {
                objectToSpawn.SetActive(true);
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
                IPoolableObject poolable = objectToSpawn.GetComponent<IPoolableObject>();
                poolable?.OnSpawn();
            }

            return objectToSpawn;
        }

        public void ReturnToPool(string tag, GameObject obj)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                return;
            }

            IPoolableObject poolable = obj.GetComponent<IPoolableObject>();
            poolable?.OnDespawn();
            obj.SetActive(false);
            Pool poolConfig = _pools.Find(p => p.tag == tag);

            if (poolConfig != null && poolConfig.parent != null)
            {
                obj.transform.SetParent(poolConfig.parent);
            }
        }

        public int GetActiveCount(string tag)
        {
            if (!_poolDictionary.ContainsKey(tag)) return 0;
            return _poolDictionary[tag].Count(obj => obj.activeInHierarchy);
        }

        public int GetTotalCount(string tag)
        {
            if (!_poolDictionary.ContainsKey(tag)) return 0;
            return _poolDictionary[tag].Count;
        }
    }

    public interface IPoolableObject
    {
        void OnSpawn();
        void OnDespawn();
    }
}
