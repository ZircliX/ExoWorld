using UnityEngine;

namespace OverBang.ExoWorld.Core.Player
{
    public class PlayerReferences : MonoBehaviour
    {
        [field : SerializeField] public Transform PlayerTransform { get; private set; }
    }
}