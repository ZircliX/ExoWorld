using System;
using OverBang.ExoWorld.Gameplay.Abilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class ShockGrenadeGadget : IGadget<ShockGrenadeData>
    {
        public ShockGrenadeData Data { get; private set; }
        public ICaster Caster { get; private set; }
        public Action OnGadgetEnded { get; }

        private ShockGrenade grenade;
        
        public void Initialize(ShockGrenadeData data, ICaster caster)
        {
            Data = data;
            Caster = caster;
        }
        
        public void Begin()
        {
            grenade = Object.Instantiate(Data.Prefab, Caster.transform.position, Quaternion.identity);
            grenade.Initialize(Data, Caster.Forward);
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