using Eflatun.SceneReference;
using OverBang.ExoWorld.Core.Metrics;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Scene
{
    [System.Serializable]
    public struct SceneCollection
    {
        public static SceneCollection Global => GameMetrics.Global.SceneCollection;
        
        [field: SerializeField]
        public SceneReference HubSceneRef { get; private set; }
        [field: SerializeField]
        public SceneReference GameSceneRef { get; private set; }
        [field: SerializeField]
        public SceneReference MainMenuSceneRef { get; private set; }
    }
}