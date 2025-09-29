using System.Collections.Generic;
using Helteix.Singletons;
using Helteix.Singletons.MonoSingletons;

namespace OverBang.GameName.Gameplay.Movement
{
    public class GravityManager : MonoSingleton<GravityManager>
    {
        private List<GravityZone> GravityZones;

        protected override void OnAwake()
        {
            //DontDestroyOnLoad(this); // Pourquoi Lupeni a fait ça ?
            base.OnAwake();
            GravityZones = new List<GravityZone>(64);
        }

        private void FixedUpdate()
        {
            foreach (GravityZone gravityZone in GravityZones)
            {
                gravityZone.OnFixedUpdate();
            }
        }

        public void RegisterGravityZone(GravityZone zone)
        {
            if (zone == null) return;
            if (GravityZones.Contains(zone)) return;

            GravityZones.Add(zone);
        }
        
        public void UnregisterGravityZone(GravityZone zone)
        {
            if (zone == null) return;
            if (!GravityZones.Contains(zone)) return;

            GravityZones.Remove(zone);
        }
        
        public void ClearGravityZones()
        {
            GravityZones.Clear();
        }
    }
}