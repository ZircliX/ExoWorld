using System;
using OverBang.GameName.Core;
using OverBang.GameName.Hub;
using Unity.Services.Multiplayer;

namespace OverBang.GameName.Gameplay
{
    public class UpgradeListener : MonoPhaseListener<HubPhase>
    {
        protected override void OnBegin(HubPhase phase)
        {
            phase.OnCharacterSelected += InitializeUpgrades;
        }

        protected override void OnEnd(HubPhase phase)
        {
            phase.OnCharacterSelected -= InitializeUpgrades;
        }

        private void InitializeUpgrades(IPlayer player, CharacterData character, bool changed)
        {
            if(changed) UpgradeManager.Instance.InitializeUpgrades(player);
        }
    }
}