using UnityEngine;
using UnityEngine.SceneManagement;

namespace LTX.Singletons
{
    public static class SceneSingletonExtensions
    {
        public static bool TryGetSceneSingleton<T>(this Scene scene, out T singleton) where T : SceneSingleton<T>
        {
            return SceneSingleton<T>.TryGetInstanceForScene(scene, out singleton);
        }
        public static bool TryGetSceneSingleton<T>(this GameObject gameObject, out T singleton) where T : SceneSingleton<T>
        {
            return SceneSingleton<T>.TryGetInstanceForScene(gameObject.scene, out singleton);
        }

    }
}