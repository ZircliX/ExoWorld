using System;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public interface IExplosionStrategy
    {
        event Action<bool> OnExploded;
        void Explode(Func<Collider[]> getOverlapColliders);
    }
}