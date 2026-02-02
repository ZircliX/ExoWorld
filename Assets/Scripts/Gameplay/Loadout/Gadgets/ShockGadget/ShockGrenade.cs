using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using Object = UnityEngine.Object;

namespace OverBang.ExoWorld.Gameplay.Loadout.ShockGadget
{
    public class ShockGrenade : IGadget
    {
        public ShockGrenadeData Data { get; private set; }
        public ICaster Caster { get; private set; }
        public event Action OnGadgetEnded;

        private ShockGrenadeEntity grenadeEntity;
        private bool isLaunched;
        

        public void Initialize(GadgetData data)
        {
            Data = data as ShockGrenadeData;
        }
        
        public void Begin(ICaster caster)
        {
            Caster = caster;

            grenadeEntity = Object.Instantiate(Data.Prefab, Caster.CastAnchor);
            grenadeEntity.FreezeGrenade(true);
        }

        public void Launch(ICaster caster)
        {
            grenadeEntity.Initialize(Data, caster.CastAnchor.forward);
            isLaunched = true;
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
            OnGadgetEnded?.Invoke();
        }
    }
}