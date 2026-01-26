using System;
using OverBang.ExoWorld.Gameplay.Abilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OverBang.ExoWorld.Gameplay
{
    public class ShockGrenadeGadget : IGadget<ShockGrenadeData>
    {
        public ShockGrenadeData DataT { get; private set; }
        public GadgetData Data { get; private set; }
        public ICaster Caster { get; private set; }
        public Action OnGadgetEnded { get; }

        private ShockGrenade grenade;
        
        public void Initialize(ShockGrenadeData data, ICaster caster)
        {
            DataT = data;
            Caster = caster;
        }
        
        public void Begin()
        {
            grenade = Object.Instantiate(DataT.Prefab, Caster.transform.position, Quaternion.identity);
            grenade.Initialize(DataT, Caster.Forward);
        }

        public void Tick(float deltaTime)
        {
            if (grenade != null)
            {
                grenade.Tick(deltaTime);
            }
        }

        public void End()
        {
            OnGadgetEnded?.Invoke();
        }
    }
}