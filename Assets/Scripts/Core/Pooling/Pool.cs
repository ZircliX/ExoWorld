using System.Collections.Generic;
using UnityEngine;

namespace OverBang.GameName.Core.Pooling
{
    public class Pool
    {
        public PoolConfig PoolConfig { get; private set; }
        public Queue<GameObject> Objects { get; private set; }
        public Transform Parent{ get; private set; }

        public Pool(Transform parent, PoolConfig poolConfig)
        {
            PoolConfig = poolConfig;
            Objects = new Queue<GameObject>(poolConfig.InitialSize);
            
            Parent = UnityEngine.Object.Instantiate(new GameObject(poolConfig.PoolName).transform, parent);
        }
        
        public void WarmUp()
        {
            for (int i = 0; i < PoolConfig.InitialSize; i++)
            {
                GameObject obj = Object.Instantiate(PoolConfig.PrefabRef, Parent);
                obj.SetActive(false);
                Objects.Enqueue(obj);
            }
        }
        
        public T GetObject<T>() where T : Component
        {
            switch (Objects.Count)
            {
                case > 0:
                {
                    GameObject obj = Objects.Dequeue();
                    Debug.Log(obj);
                    obj.gameObject.SetActive(true);
                    return obj.GetComponent<T>();
                }
                case 0 when PoolConfig.Expendable:
                {
                    GameObject obj = UnityEngine.Object.Instantiate(PoolConfig.PrefabRef, Parent);
                    return obj.GetComponent<T>();
                }
            }

            Debug.Log("No object found");
            return null;
        }
        
        public void ReturnObject(GameObject obj)
        {
            obj.SetActive(false);
            Objects.Enqueue(obj);
        }
        
        public void Clear()
        {
            while (Objects.Count > 0)
                Object.Destroy(Objects.Dequeue());
        }
    }
}