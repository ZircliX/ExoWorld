using UnityEngine;

namespace LTX.Singletons
{
    public class MonoSingletonFinder<T> : ISingletonFinder<T> where T : MonoBehaviour, ISingleton
    {
        public bool TryFindExistingInstance(out T instance)
        {
            instance = GameObject.FindFirstObjectByType<T>();
            return instance != null;
        }
    }
}