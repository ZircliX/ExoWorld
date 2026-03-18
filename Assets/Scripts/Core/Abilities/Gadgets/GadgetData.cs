using Ami.BroAudio;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Abilities.Gadgets
{
    public abstract class GadgetData : ScriptableObject
    {
        [field: Header("General Data")]
        
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        
        [field: SerializeField] public SoundID SoundID { get; private set; }
        [field: SerializeField] public NetworkObject ExplosionEffect { get; private set; }
        [field: SerializeField] public float ExplosionDelay { get; private set; }
        [field: SerializeField] public float ExplosionRadius { get; private set; }
    }
}