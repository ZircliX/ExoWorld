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

        public bool IsEquiped { get; private set; }
        public bool IsCasting { get; private set; }
        public event Action OnGadgetEnded;
        
        public event Action OnGadgetBeingCasted;
        
        private BurstGrenadeEntity grenadeEntity;
        private bool isLaunched;

        public BurstGrenade(BurstGrenadeData burstGrenadeData)
        {
            Data = burstGrenadeData;
        }
        
        public void Begin(ICaster caster)
        {
            isLaunched = false;
            Caster = caster;
            IsEquiped = true;
            IsCasting = false;
            
            
            grenadeEntity = Object.Instantiate(Data.Prefab, Caster.CastAnchor);
            grenadeEntity.FreezeGrenade(true);
        }

        public void Cast(ICaster caster)
        {
            isLaunched = true;
            IsCasting = true;
            grenadeEntity.Initialize(Data, caster.CastAnchor.forward, this);
        }

        public void Tick(float deltaTime)
        {
            if (!isLaunched) return;
            if (grenadeEntity != null)
            {
                grenadeEntity.Tick(deltaTime);
            }
        }

        public void End()
        {
            IsEquiped = false;
            IsCasting = false;
            isLaunched = false;
            OnGadgetEnded?.Invoke();
        }

        public void Discard()
        {
            Object.Destroy(grenadeEntity.gameObject);
            grenadeEntity = null;
            End();
        }
    }
}