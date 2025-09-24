using System;
using System.Collections;
using System.Collections.Generic;
using LTX.Singletons.Settings;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LTX.Singletons
{
    public abstract partial class SceneSingleton<T> : MonoBehaviour, ISingleton where T : SceneSingleton<T>
    {
        private void Awake()
        {
            if (Register(this))
            {
                OnSceneSingletonAwake();
            }
            else
            {
                gameObject.SetActive(false);
                if (IsCrossSceneSingleton() && TryGetInstanceInAnyScene(out T existing))
                {
                    Debug.LogWarning($"A singleton of type {typeof(T).Name} is already present in one of the loaded scene. Only one MultiSceneSingleton can exist at the same time.", existing);
                }
                else if(TryGetInstanceForScene(gameObject.scene, out existing))
                {
                    Debug.LogWarning($"A singleton of type {typeof(T).Name} is already present in the scene. Only one SceneSingleton can exist at the same time.", existing);
                }
                else
                {
                    Debug.LogWarning($"Could not register scene singleton for scene {gameObject.name}.", this);
                }
            }
        }

        private void OnDestroy()
        {
            if (Unregister(this))
            {
                OnSceneSingletonDestroy();
            }
            else
            {
                Debug.LogWarning($"Could not unregister scene singleton for scene {gameObject.name}", this);
            }
        }

        protected virtual void OnSceneSingletonAwake() { }
        protected virtual void OnSceneSingletonDestroy() { }

        protected virtual T GetSingleton() => this as T;
    }
}