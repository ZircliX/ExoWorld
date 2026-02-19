using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using Object = UnityEngine.Object;

namespace OverBang.ExoWorld.Gameplay.Loadout.ParalixGadget
{
    public class ParalixGrenade : IGadget
    {
        GadgetData IGadget.Data => Data;
        public ParalixGrenadeData Data { get; private set; }
        
        public ICaster Caster { get; private set; }

        public bool IsEquiped { get; private set; }
        public bool IsCasting { get; private set; }
        public event Action<IGadget> OnGadgetCasted;
        public event Action<IGadget> OnGadgetEnded;
        
        private ParalixGrenadeEntity grenadeEntity;
        private bool isLaunched;

        public ParalixGrenade(ParalixGrenadeData GrenadeData)
        {
            Data = GrenadeData;
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
            Reset();
            OnGadgetEnded?.Invoke(this);
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