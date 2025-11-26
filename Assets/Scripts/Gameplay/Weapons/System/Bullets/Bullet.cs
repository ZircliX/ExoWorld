using OverBang.Pooling;
using OverBang.Pooling.Resource;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public abstract class Bullet : MonoBehaviour, IPoolInstanceListener
    {
        [SerializeField] protected LayerMask hitLayers;
        public abstract IPool Pool { get; protected set; }
        
        public abstract void Fire(Transform origin, Vector3 direction, float speed);

        public abstract void OnSpawn(IPool pool);
        public abstract void OnDespawn(IPool pool);
    }
}