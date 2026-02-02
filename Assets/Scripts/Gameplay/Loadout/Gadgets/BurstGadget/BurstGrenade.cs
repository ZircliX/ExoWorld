using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OverBang.ExoWorld.Gameplay.Loadout.BurstGadget
{
    public class BurstGrenade : IGadget
    {
        
        public BurstGrenadeData Data { get; private set; }
        
        public ICaster Caster { get; private set; }
        
        public event Action OnGadgetEnded;
        
        private BurstGrenadeEntity grenadeEntity;
        
        
        public void Initialize(GadgetData data)
        {
            Data = data as BurstGrenadeData;
        }

        public void Begin(ICaster caster)
        {
            Caster = caster;
            grenadeEntity = Object.Instantiate(Data.Prefab, Caster.CastAnchor.position, Quaternion.identity);
            grenadeEntity.FreezeGrenade(true);
        }

        public void Launch(ICaster caster)
        {
            grenadeEntity.Initialize(Data, caster.Forward);
        }

        public void Tick(float deltaTime)
        {
            if (grenadeEntity != null)
            {
                grenadeEntity.Tick(deltaTime);
            }
        }

        public void End()
        {
            OnGadgetEnded?.Invoke();
        }
    }
}