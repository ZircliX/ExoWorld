using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    public struct PlayerRuntimeContext
    {
        public PlayerController playerController;
        public CharacterData playerCharacterData;
        public Animator playerAnimator;
        public PlayerRig PlayerRig;
    }
}