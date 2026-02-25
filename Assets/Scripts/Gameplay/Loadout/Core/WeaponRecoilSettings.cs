using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    [CreateAssetMenu(menuName = "OverBang/Weapons/WeaponRecoilSettings")]
    public class WeaponRecoilSettings : ScriptableObject
    {
        [field: SerializeField] public float RecoilX { get; private set; }
        [field: SerializeField] public float RecoilY { get; private set; }
        [field: SerializeField] public float RecoilZ { get; private set; }

        [field: SerializeField] public float Snappiness { get; private set; }
        [field: SerializeField] public float ReturnSpeed { get; private set; }

        public Vector3 GetRecoil()
        {
            Vector3 recoil = new Vector3(
                RecoilX,
                Random.Range(-RecoilY, RecoilY),
                Random.Range(-RecoilZ, RecoilZ)
            );

            return recoil;
        }
    }
}