using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Gameplay.IK_Animation;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Player
{
    public struct PlayerRuntimeContext
    {
        public PlayerController playerController;
        public CharacterData playerCharacterData;
        public Animator playerAnimator;
        public PlayerRig PlayerRig;
    }
}