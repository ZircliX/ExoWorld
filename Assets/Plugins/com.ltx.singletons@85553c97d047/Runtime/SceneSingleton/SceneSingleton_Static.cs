using System;
using System.Collections;
using System.Collections.Generic;
using LTX.Singletons.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LTX.Singletons
{
    public abstract partial class SceneSingleton<T>
    {
        private static List<T> singletons;

        static SceneSingleton()
        {
            singletons = new();
        }

        public static bool TryGetInstanceForScene(Scene scene, out T singleton)
        {
            foreach (var s in singletons)
            {
                if (s.gameObject.scene == scene)
                {
                    singleton = s;
                    return true;
                }
            }

            singleton = null;
            return false;
        }

        public static bool TryGetInstanceInActiveScene(out T singleton) => TryGetInstanceForScene(SceneManager.GetActiveScene(), out singleton);
        public static bool TryGetInstanceInAnyScene(out T singleton)
        {
            foreach (T value in singletons)
            {
                if (value != null)
                {
                    singleton = value;
                    return true;
                }
            }

            singleton = null;
            return false;
        }

        public static T GetInstanceForScene(Scene scene) => TryGetInstanceForScene(scene, out T singleton) ? singleton : null;

        public static T GetInstanceInActiveScene()
        {
            if (TryGetInstanceInActiveScene(out T singleton))
                return singleton;

            return null;
        }

        public static T GetInstanceInAnyScene()
        {
            if (TryGetInstanceInAnyScene(out T singleton))
                return singleton;

            return null;
        }
        public static bool IsCrossSceneSingleton() => SceneSingletonUtility.IsCrossScene<T>();

        /// <summary>
        /// Checks for an overall instance and if none was found, returns null.
        /// Like the property instance, this property doesn't look in all scenes if the scene singleton isn't cross-scene.
        /// <seealso cref="Instance"/>
        /// </summary>
        public static bool HasInstance => Instance != null;

        /// <summary>
        /// If the scene singleton is not cross scene, the returned value is the singleton from the active scene.
        /// If none is found, the result is null.
        /// For further control, please use the <see cref="TryGetInstanceForScene"/> method.
        /// <seealso cref="TryGetInstanceInAnyScene"/>
        /// <seealso cref="TryGetInstanceInActiveScene"/>
        /// <seealso cref="TryGetInstanceInActiveScene"/>
        /// </summary>
        public static T Instance => IsCrossSceneSingleton() ? GetInstanceInAnyScene() : GetInstanceInActiveScene();

        private static bool Register(SceneSingleton<T> sceneSingleton)
        {
            Scene gameObjectScene = sceneSingleton.gameObject.scene;
            if (IsCrossSceneSingleton())
            {
                if (singletons.Count > 0)
                    return false;
            }
            else if (singletons.Exists(ctx => ctx.gameObject.scene == gameObjectScene))
                return false;

            Debug.Log($"Registering {sceneSingleton.name} as a SceneSingleton");
            singletons.Add(sceneSingleton.GetSingleton());
            return true;
        }


        private static bool Unregister(SceneSingleton<T> sceneSingleton)
        {
            return singletons.Remove(sceneSingleton.GetSingleton());
        }
    }
}