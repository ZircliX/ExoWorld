using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Abilities.Gadgets;
using OverBang.ExoWorld.Core.GameMode.Players;
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
        public event Action<IGadget> OnGadgetEnded;
        
        private BurstGrenadeEntity grenadeEntity;
        private bool isLaunched;

        public BurstGrenade(BurstGrenadeData burstGrenadeData)
        {
            Data = burstGrenadeData;
        }
        
        public void Begin(ICaster caster, LocalGamePlayer player)
        {
            Caster = caster;
            isLaunched = false;
            IsEquiped = true;
            IsCasting = false;
            grenadeEntity = Object.Instantiate(Data.Prefab, Caster.CastAnchor);
            grenadeEntity.FreezeGrenade(true);
        }

        public void Cast(Camera cam)
        {
            isLaunched = true;
            IsCasting = true;
            grenadeEntity.Initialize(Data, cam.transform.forward, this);
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