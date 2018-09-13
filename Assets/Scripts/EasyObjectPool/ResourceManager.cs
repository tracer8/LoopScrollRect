using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace SG
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class ResourceManager : MonoBehaviour
    {
        //obj pool
        private Dictionary<int, Pool> poolDict = new Dictionary<int, Pool>();

        private static ResourceManager mInstance = null;

        public static ResourceManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject go = new GameObject("ResourceManager", typeof(ResourceManager));
                    go.transform.localPosition = new Vector3(9999999, 9999999, 9999999);
                    // Kanglai: if we have `GO.hideFlags |= HideFlags.DontSave;`, we will encounter Destroy problem when exit playing
                    // However we should keep using this in Play mode only!
                    mInstance = go.GetComponent<ResourceManager>();

                    if (Application.isPlaying)
                    {
                        DontDestroyOnLoad(mInstance.gameObject);
                    }
                    else
                    {
                        Debug.LogWarning("[ResourceManager] You'd better ignore ResourceManager in Editor mode");
                    }
                }

                return mInstance;
            }
        }
        public void InitPool(GameObject prefab, int size, PoolInflationType type = PoolInflationType.DOUBLE)
        {
            int key = prefab.GetInstanceID();

            if (poolDict.ContainsKey(key))
            {
                return;
            }
            else
            {
                GameObject pb = Instantiate(prefab);

                if (pb == null)
                {
                    Debug.LogError("[ResourceManager] Invalide prefab name for pooling :" + prefab.name);
                    return;
                }

                poolDict[key] = new Pool(key, pb, gameObject, size, type);
            }
        }

        /// <summary>
        /// Returns an available object from the pool 
        /// OR null in case the pool does not have any object available & can grow size is false.
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public GameObject GetObjectFromPool(GameObject prefab, bool autoActive = true, int autoCreate = 0)
        {
            int key = prefab.GetInstanceID();
            GameObject result = null;

            if (!poolDict.ContainsKey(key) && autoCreate > 0)
            {
                InitPool(prefab, autoCreate, PoolInflationType.INCREMENT);
            }

            if (poolDict.ContainsKey(key))
            {
                Pool pool = poolDict[key];
                result = pool.NextAvailableObject(autoActive);
                //scenario when no available object is found in pool
#if UNITY_EDITOR
                if (result == null)
                {
                    Debug.LogWarning("[ResourceManager]:No object available in " + prefab.name);
                }
#endif
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("[ResourceManager]:Invalid pool name specified: " + prefab.name);
            }
#endif
            return result;
        }

        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <param name="go"></param>
        public void ReturnObjectToPool(GameObject go)
        {
            PoolObject po = go.GetComponent<PoolObject>();
            if (po == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Specified object is not a pooled instance: " + go.name);
#endif
            }
            else
            {
                Pool pool = null;
                if (poolDict.TryGetValue(po.poolId, out pool))
                {
                    pool.ReturnObjectToPool(po);
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogWarning("No pool available with name: " + po.poolId);
                }
#endif
            }
        }

        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <param name="t"></param>
        public void ReturnTransformToPool(Transform t)
        {
            if (t == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[ResourceManager] try to return a null transform to pool!");
#endif
                return;
            }
            ReturnObjectToPool(t.gameObject);
        }

        public GameObject Create(GameObject prefab)
        {
            return Instantiate(prefab);
        }

        public void Remove(GameObject go)
        {
            Destroy(go);
        }
    }
}