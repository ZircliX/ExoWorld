using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public struct PlayerRuntimeContext
    {
        public PlayerController playerController;
        public CharacterData playerCharacterData;
        public Animator playerAnimator;
        public PlayerRig PlayerRig;
    }
}