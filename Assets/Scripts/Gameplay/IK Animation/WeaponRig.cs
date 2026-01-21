using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    public class WeaponRig : MonoBehaviour
    {
        [field: SerializeField] public Transform shootPoint { get; private set; }
        [field: SerializeField] public Transform targetHandR { get; private set; }
        [field: SerializeField] public Transform targetHandL { get; private set; }
    }
}