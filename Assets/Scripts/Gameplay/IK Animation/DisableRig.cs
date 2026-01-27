using OverBang.ExoWorld.Gameplay.Loadout;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace OverBang.ExoWorld.Gameplay.IK_Animation
{
    public class DisableRig : MonoBehaviour
    {
        [SerializeField] private WeaponController wc;

        private void OnEnable()
        {
            wc.OnRigBuilderAccessed += OnDisableRig;
        }

        private void OnDisable()
        {
            wc.OnRigBuilderAccessed -= OnDisableRig;
        }

        private void OnDisableRig(RigBuilder rigBuilder)
        {
            foreach (RigLayer layer in rigBuilder.layers)
            {
                layer.rig.weight = 0;
            }
        }
    }
}