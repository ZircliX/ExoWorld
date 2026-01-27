using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Gameplay.Phase;

namespace OverBang.ExoWorld.Gameplay.Upgrade
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

        private void InitializeUpgrades(LocalGamePlayer player, bool changed)
        {
            if(changed) UpgradeManager.Instance.InitializeUpgrades(player);
        }
    }
}