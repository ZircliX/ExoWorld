using UnityEngine;

namespace OverBang.GameName.Gameplay.Combat
{
    public abstract class Bullet : MonoBehaviour
    {
        public abstract void Fire(Vector3 direction, float speed);
        protected abstract void Release(Collider other);

        public virtual void OnTriggerEnter(Collider other)
        {
            Release(other);
        }
    }
}