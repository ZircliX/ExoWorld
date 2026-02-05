using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using Object = UnityEngine.Object;

namespace OverBang.ExoWorld.Gameplay.Loadout.ShockGadget
{
    public class ShockGrenade : IGadget
    {
        GadgetData IGadget.Data => Data;
        public ShockGrenadeData Data { get; private set; }
        public ICaster Caster { get; private set; }
        public bool IsEquiped { get; private set; }
        public bool IsCasting { get; private set; }
        public event Action OnGadgetEnded;
        public event Action OnGadgetBeingCasted;

        private ShockGrenadeEntity grenadeEntity;
        private bool isLaunched;

        public ShockGrenade(ShockGrenadeData shockGrenadeData)
        {
            Data = shockGrenadeData;
        }
        
        public void Begin(ICaster caster)
        {
            isLaunched = false;
            Caster = caster;
            IsEquiped = true;
            IsCasting = false;
            OnGadgetBeingCasted?.Invoke();
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