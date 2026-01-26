using System;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class EmptyExplosion : IExplosionStrategy
    {
        public event Action<bool> OnExploded;

        public EmptyExplosion()
        {
   
        }

        public void Explode(Func<Collider[]> getOverlapColliders)
        {
            OnExploded?.Invoke(true);
        }
    }
}