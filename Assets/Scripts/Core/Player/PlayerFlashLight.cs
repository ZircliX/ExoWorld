using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Scene;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Player
{
    public class PlayerFlashLight : MonoBehaviour
    {
        [SerializeField] private Light flashLight;

        private void OnEnable()
        {
            SetState(SceneLoader.GetCurrentScene().name != GameMetrics.Global.SceneCollection.HubSceneRef.Name);
        }

        private void SetState(bool state)
        {
            flashLight.enabled = state;
        }
    }
}