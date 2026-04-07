using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Scene;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Player
{
    public class PlayerFlashLight : MonoBehaviour
    {
        [SerializeField] private Light flashLight;

        private void Awake()
        {
            if (flashLight == null) flashLight = GetComponentInChildren<Light>();

            bool state = SceneLoader.GetCurrentScene().name != GameMetrics.Global.SceneCollection.HubSceneRef.Name;
            SetState(state);
        }

        private void SetState(bool state)
        {
            if (flashLight == null) return;
            flashLight.enabled = state;
        }
    }
}