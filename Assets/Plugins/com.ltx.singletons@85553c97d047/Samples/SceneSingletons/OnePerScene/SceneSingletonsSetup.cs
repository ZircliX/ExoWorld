using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LTX.Singletons
{
    public class SceneSingletonsSetup : MonoBehaviour
    {
        [SerializeField]
        private int sceneCount;

        private Scene[] scenes;

        private void OnEnable()
        {
            scenes = new Scene[sceneCount];
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.CreateScene($"SampleScene_{i}", new CreateSceneParameters(LocalPhysicsMode.None));

                GameObject container = new GameObject($"Singleton {i}");
                SceneManager.MoveGameObjectToScene(container, scene);
                container.AddComponent<SampleSceneSingleton>();

                scenes[i] = scene;
            }
        }

        private void OnGUI()
        {
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = scenes[i];
                var height = 20;
                var offset = 15;
                var width = 350;
                Rect rect = new Rect()
                {
                    x = offset,
                    y = offset + height * i + 5 * i,
                    width = width,
                    height = height
                };

                if (GUI.Button(rect, $"Log for scene {scene.name}"))
                {
                    if (scene.TryGetSceneSingleton(out SampleSceneSingleton sceneSingleton))
                        sceneSingleton.Log();
                }
            }
        }
    }
}