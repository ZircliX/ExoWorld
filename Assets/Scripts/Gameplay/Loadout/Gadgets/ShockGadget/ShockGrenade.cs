using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OverBang.ExoWorld.Gameplay.Loadout.ShockGadget
{
    public class ShockGrenade : IGadget
    {
        public ShockGrenadeData Data { get; private set; }
        public ICaster Caster { get; private set; }
        public Action OnGadgetEnded { get; }

        private ShockGrenadeEntity grenadeEntity;
        
        public void Initialize(GadgetData data)
        {
            Debug.Log(data.GetType() ==  typeof(ShockGrenadeData));
            Data = data as ShockGrenadeData;
            grenadeEntity.FreezeGrenade(true);
        }
        
        public void Begin(ICaster caster)
        {
            Caster = caster;
            grenadeEntity = Object.Instantiate(Data.Prefab, Caster.CastAnchor.position, Quaternion.identity);
        }

        public void Launch(ICaster caster)
        {
            grenadeEntity.Initialize(Data, caster.CastAnchor.forward);
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