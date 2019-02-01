using UnityEngine;
using System.Collections;
using Lean.Pool;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource 
    {
        public GameObject prefabObject;
        
        public virtual GameObject GetObject()
        {   
            return LeanPool.Spawn(prefabObject);
        }

        public virtual void ReturnObject(Transform go)
        {   
            LeanPool.Despawn(go.gameObject);
        }
    }
}
