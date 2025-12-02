using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class WeaponRig : MonoBehaviour
    {
        [field: SerializeField] public Transform targetHandR { get; private set; }
        [field: SerializeField] public Transform targetHandL { get; private set; }
    }
}