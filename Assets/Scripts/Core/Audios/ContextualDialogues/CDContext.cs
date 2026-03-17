using OverBang.ExoWorld.Core.Characters;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Audios.ContextualDialogues
{
    public struct CDContext
    {
        public CharacterData data;
        public ulong playerId;
        
        public Transform sourceTransform;
        public Vector3 sourcePosition;
        public SourceType sourceType;
        
        public enum SourceType
        {
            FollowSpatialized,
            FixedSpatialized,
            Static
        }
    }
}