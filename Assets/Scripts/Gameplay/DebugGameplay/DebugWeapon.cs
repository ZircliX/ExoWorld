using OverBang.Pooling;
using OverBang.Pooling.Resource;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay.Gameplay.DebugGameplay
{
    public class DebugWeapon : MonoBehaviour
    {
        [SerializeField] private Transform shootPoint;
        [SerializeField] private PoolResource bulletResource;

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                GameObject bullet = bulletResource.Spawn<GameObject>();
                bullet?.GetComponent<DebugBullet>()?.Fire(transform);
            }
        }
    }
}