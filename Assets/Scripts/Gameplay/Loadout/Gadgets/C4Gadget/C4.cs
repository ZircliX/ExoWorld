using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using Object = UnityEngine.Object;


namespace OverBang.ExoWorld.Gameplay.Loadout.C4Gadget
{
    public class C4 : IGadget
    {
        GadgetData IGadget.Data => Data;
        public C4Data Data { get; private set; }
        public ICaster Caster { get; private set; }

        public bool IsEquiped { get; private set; }
        public bool IsCasting { get; private set; }
        public event Action<IGadget> OnGadgetEnded;
        
        private C4Entity grenadeEntity;
        private bool isLaunched;

        public C4(C4Data c4GadgetData)
        {
            this.Data = c4GadgetData;
        }
        
        public void Begin(ICaster caster)
        {
            Caster = caster;
            isLaunched = false;
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