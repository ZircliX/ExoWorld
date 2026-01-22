using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public interface ICaster
    {
        Transform transform { get; }
        GameObject gameObject { get; }
        Vector3 Forward { get; }
    }
}