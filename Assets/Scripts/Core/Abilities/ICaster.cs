using UnityEngine;

namespace OverBang.ExoWorld.Core.Abilities
{
    public interface ICaster
    {
        Transform transform { get; }
        Transform CastAnchor { get; }
        GameObject gameObject { get; }
        Vector3 Forward { get; }
    }
}