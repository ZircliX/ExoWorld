using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using Object = UnityEngine.Object;

namespace OverBang.ExoWorld.Gameplay.Loadout.FrostBiteGadget
{
    public class FrostBiteGrenade : IGadget
    {
        GadgetData IGadget.Data => Data;
        public FrostBiteGrenadeData Data { get; private set; }
        
        public ICaster Caster { get; private set; }

        public bool IsEquiped { get; private set; }
        public bool IsCasting { get; private set; }
        public event Action<IGadget> OnGadgetCasted;
        public event Action<IGadget> OnGadgetEnded;
        
        private FrostBiteGrenadeEntity grenadeEntity;
        private bool isLaunched;

        public FrostBiteGrenade(FrostBiteGrenadeData GrenadeData)
        {
            Data = GrenadeData;
        }
        
        public void Begin(ICaster caster)
        {
            isLaunched = false;
            Caster = caster;
            IsEquiped = true;
            IsCasting = false;
            OnGadgetCasted?.Invoke(this);
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