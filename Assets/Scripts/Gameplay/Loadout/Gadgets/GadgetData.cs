using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public abstract class GadgetData : ScriptableObject
    {
        [field: Header("General Data")]
        
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public float ExplosionDelay { get; private set; }
        [field: SerializeField] public float ExplosionRadius { get; private set; }
        
    }
}