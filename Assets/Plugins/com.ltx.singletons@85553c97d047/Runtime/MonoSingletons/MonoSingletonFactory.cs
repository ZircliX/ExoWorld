
using UnityEngine;

namespace LTX.Singletons
{
    public class MonoSingletonFactory<T> : ISingletonFactory<T> where T : MonoBehaviour, ISingleton
    {
        public virtual T CreateSingleton()
        {
            string fullName = typeof(T).ToString();
            var nameArray = fullName.Split('.');
            string name = $"{nameArray[^1]} Instance";
            var singletonObject = new GameObject(name);
            Debug.Log($"{name} was created because none was found", singletonObject);
            return singletonObject.AddComponent<T>();
        }
    }
}