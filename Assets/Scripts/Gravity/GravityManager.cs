using System.Collections.Generic;
using LTX.Singletons;

namespace DeadLink.Gravity
{
    public class GravityManager : MonoSingleton<GravityManager>
    {
        private List<GravityZone> GravityZones;

        protected override void Awake()
        {
            //DontDestroyOnLoad(this); // Pourquoi Lupeni a fait ça ?
            base.Awake();
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