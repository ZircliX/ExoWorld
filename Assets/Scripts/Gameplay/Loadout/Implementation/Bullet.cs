using OverBang.Pooling;
using OverBang.Pooling.Resource;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public abstract class Bullet : NetworkBehaviour, IPoolInstanceListener
    {
        public abstract IPool Pool { get; protected set; }
        
        public abstract void Fire(Vector3 vector3, Vector3 direction, BulletData bulletData);

        public abstract void OnSpawn(IPool pool);
        public abstract void OnDespawn(IPool pool);
    }
}