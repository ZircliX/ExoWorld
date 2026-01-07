using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public interface IAbilityCaster
    {
        Transform transform { get; }
        GameObject gameObject { get; }
        Vector3 Forward { get; }
    }
}