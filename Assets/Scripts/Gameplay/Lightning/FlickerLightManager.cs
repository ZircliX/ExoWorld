using System.Collections.Generic;
using Helteix.Singletons.SceneServices;
using Helteix.Tools;

namespace OverBang.ExoWorld.Gameplay.Lightning
{
    public class FlickerLightManager : SceneService<FlickerLightManager>
    {
        private List<FlickeringLight> lights;
        private DynamicBuffer<FlickeringLight> buffer;

        protected override void Activate()
        {
            lights = new List<FlickeringLight>(32);
            buffer = new DynamicBuffer<FlickeringLight>(32);
        }

        public void RegisterLight(FlickeringLight light)
        {
            lights.Add(light);
        }

        public void UnregisterLight(FlickeringLight light)
        {
            lights.Remove(light);
        }

        public void ClearLights()
        {
            lights.Clear();
        }

        private void Update()
        {
            buffer.Clear();
            buffer.CopyFrom(lights);

            for (int index = 0; index < buffer.Length; index++)
            {
                FlickeringLight light = buffer[index];
                light.FlickerLight();
            }
        }
    }

    public static class FlickerLightUtils
    {
        public static void Register(this FlickeringLight light)
        {
            FlickerLightManager.Instance.RegisterLight(light);
        }
        
        public static void Unregister(this FlickeringLight light)
        {
            FlickerLightManager.Instance.UnregisterLight(light);
        }
    }
}