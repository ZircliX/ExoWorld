using OverBang.ExoWorld.Core;
using Unity.Services.Multiplayer;

namespace OverBang.ExoWorld.Gameplay
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