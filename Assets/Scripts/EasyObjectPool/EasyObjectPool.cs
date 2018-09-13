/* 
 * Unless otherwise licensed, this file cannot be copied or redistributed in any format without the explicit consent of the author.
 * (c) Preet Kamal Singh Minhas, http://marchingbytes.com
 * contact@marchingbytes.com
 */
// modified version by Kanglai Qian
using UnityEngine;
using System.Collections.Generic;

namespace SG
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class PoolObject : MonoBehaviour
    {
        public int poolId;
        //defines whether the object is waiting in pool or is in use
        public bool isPooled;
    }

    public enum PoolInflationType
    {
        /// When a dynamic pool inflates, add one to the pool.
        INCREMENT,
        /// When a dynamic pool inflates, double the size of the pool
        DOUBLE
    }

    class Pool
    {
        private Stack<PoolObject> availableObjStack = new Stack<PoolObject>();

        //the root obj for unused obj
        private GameObject rootObj;
        private PoolInflationType inflationType;
        private int poolId;
        private int objectsInUse = 0;
        private string poolName;

        public Pool(int poolId, GameObject poolObjectPrefab, GameObject rootPoolObj, int initialCount, PoolInflationType type)
        {
            if (poolObjectPrefab == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[ObjPoolManager] null pool object prefab !");
#endif
                return;
            }
            this.poolId = poolId;
            this.inflationType = type;
            this.rootObj = new GameObject(string.Format("Pool Key: {0}-{1}", poolObjectPrefab.name, this.poolId));
            this.rootObj.transform.SetParent(rootPoolObj.transform, false);
            this.poolName = poolObjectPrefab.name;

            // In case the origin one is Destroyed, we should keep at least one
            GameObject go = GameObject.Instantiate(poolObjectPrefab);
            PoolObject po = go.GetComponent<PoolObject>();
            if (po == null)
            {
                po = go.AddComponent<PoolObject>();
            }
            po.poolId = poolId;
            AddObjectToPool(po);

            //populate the pool
            populatePool(Mathf.Max(initialCount, 1));
        }

        //o(1)
        private void AddObjectToPool(PoolObject po)
        {
            //add to pool
            po.gameObject.SetActive(false);
            po.gameObject.name = poolName;
            availableObjStack.Push(po);
            po.isPooled = true;
            //add to a root obj
            po.gameObject.transform.SetParent(rootObj.transform, false);
        }

        private void populatePool(int initialCount)
        {
            for (int index = 0; index < initialCount; index++)
            {
                PoolObject po = GameObject.Instantiate(availableObjStack.Peek());
                AddObjectToPool(po);
            }
        }

        //o(1)
        public GameObject NextAvailableObject(bool autoActive)
        {
            PoolObject po = null;
            if (availableObjStack.Count > 1)
            {
                po = availableObjStack.Pop();
            }
            else
            {
                int increaseSize = 0;
                //increment size var, this is for info purpose only
                if (inflationType == PoolInflationType.INCREMENT)
                {
                    increaseSize = 1;
                }
                else if (inflationType == PoolInflationType.DOUBLE)
                {
                    increaseSize = availableObjStack.Count + Mathf.Max(objectsInUse, 0);
                }
#if UNITY_EDITOR
                Debug.Log(string.Format("Growing pool {0}: {1} populated", poolId, increaseSize));
#endif
                if (increaseSize > 0)
                {
                    populatePool(increaseSize);
                    po = availableObjStack.Pop();
                }
            }

            GameObject result = null;
            if (po != null)
            {
                objectsInUse++;
                po.isPooled = false;
                result = po.gameObject;
                if (autoActive)
                {
                    result.SetActive(true);
                }
            }

            return result;
        }

        //o(1)
        public void ReturnObjectToPool(PoolObject po)
        {
            if (poolId.Equals(po.poolId))
            {
                objectsInUse--;
                /* we could have used availableObjStack.Contains(po) to check if this object is in pool.
                 * While that would have been more robust, it would have made this method O(n) 
                 */
                if (po.isPooled)
                {
#if UNITY_EDITOR
                    Debug.LogWarning(po.gameObject.name + " is already in pool. Why are you trying to return it again? Check usage.");
#endif
                }
                else
                {
                    AddObjectToPool(po);
                }
            }
            else
            {
                Debug.LogError(string.Format("Trying to add object to incorrect pool {0} {1}", po.poolId, poolId));
            }
        }
    }
}