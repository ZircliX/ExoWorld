using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public interface IPlayerComponent
    {
        PlayerController Controller { get; set; }
        void OnSync(CharacterData data, Animator animator);
    }
}