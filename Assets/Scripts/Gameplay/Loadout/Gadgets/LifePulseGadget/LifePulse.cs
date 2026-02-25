using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.GameMode.Players;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OverBang.ExoWorld.Gameplay.Loadout.LifePulseGadget
{
    public class LifePulse : IGadget
    {
        GadgetData IGadget.Data => Data;
        public LifePulseData Data { get; private set; }
        public ICaster Caster { get; private set; }
        public bool IsEquiped { get; private set; }
        public bool IsCasting { get; private set; }
        
        
        public event Action<IGadget> OnGadgetEnded;
        
        private LifePulseEntity grenadeEntity;
        private bool isLaunched;
        
        public LifePulse(LifePulseData lifePulseData)
        {
            Data = lifePulseData;
        }
        
        public void Begin(ICaster caster, LocalGamePlayer player)
        {
            isLaunched = false;
            Caster = caster;
            IsEquiped = true;
            IsCasting = false;
            
            grenadeEntity = Object.Instantiate(Data.Prefab, Caster.CastAnchor);
            grenadeEntity.FreezeGrenade(true);
        }

        public void Cast(Camera cam)
        {
            isLaunched = true;
            IsCasting = true;
            grenadeEntity.Initialize(Data, this);
           
        }

        public void Tick(float deltaTime)
        {
        }

        public void End()
        {
            OnGadgetEnded?.Invoke(this);
            Reset();
        }

        public void Discard()
        {
            Object.Destroy(grenadeEntity.gameObject);
            grenadeEntity = null;
            Reset();
        }

        private void Reset()
        {
            IsEquiped = false;
            IsCasting = false;
            isLaunched = false;
        }
    }
}