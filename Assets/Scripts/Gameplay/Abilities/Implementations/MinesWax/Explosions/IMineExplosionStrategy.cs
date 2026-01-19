using System;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public interface IMineExplosionStrategy
    {
        event Action<bool> OnExploded;
        void Explode(Func<Collider[]> getOverlapColliders);
    }
}