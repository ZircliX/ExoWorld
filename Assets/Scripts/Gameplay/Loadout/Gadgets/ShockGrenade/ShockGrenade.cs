using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Abilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class ShockGrenade : IGadget<ShockGrenadeData>
    {
        public ShockGrenadeData DataT { get; private set; }
        public GadgetData Data { get; private set; }
        public ICaster Caster { get; private set; }
        public Action OnGadgetEnded { get; }

        private ShockGrenadeEntity grenadeEntity;
        
        public void Initialize(ShockGrenadeData data, ICaster caster)
        {
            DataT = data;
            Caster = caster;
        }
        
        public void Begin()
        {
            grenadeEntity = Object.Instantiate(DataT.Prefab, Caster.transform.position, Quaternion.identity);
            grenadeEntity.Initialize(DataT, Caster.Forward);
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