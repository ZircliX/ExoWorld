using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OverBang.ExoWorld.Gameplay.Loadout.BurstGadget
{
    public class BurstGrenade : IGadget
    {
        GadgetData IGadget.Data => Data;

        public BurstGrenadeData Data { get; private set; }
        
        public ICaster Caster { get; private set; }

        public bool IsEquiped { get; }
        public bool IsCasting { get; }
        public event Action OnGadgetEnded;
        
        private BurstGrenadeEntity grenadeEntity;

        public BurstGrenade(BurstGrenadeData burstGrenadeData)
        {
            Data = burstGrenadeData;
        }


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

        public void Cast(ICaster caster)
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