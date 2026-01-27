using UnityEngine;

namespace OverBang.ExoWorld.Core.Characters
{
    [System.Serializable]
    public struct CharacterBaseStats
    {
        [field: SerializeField]
        public float Health { get; private set; }
        [field: SerializeField]
        public float Resistance { get; private set; }
        
    }
}