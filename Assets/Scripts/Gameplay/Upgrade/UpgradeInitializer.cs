using OverBang.GameName.Core;
using OverBang.GameName.Hub;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class UpgradeInitializer : MonoPhaseListener<HubPhase>
    {
        protected override void OnBegin(HubPhase phase)
        {
            phase.OnCharacterSelected += OnCharacterSelected;
            
        }

        protected override void OnEnd(HubPhase phase)
        {
            phase.OnCharacterSelected += OnCharacterSelected;
        }

        private void OnCharacterSelected(IPlayer character, CharacterData data)
        {
            UpgradeManager.Instance.InitializeUpgrades(character);
            Debug.Log("Character selected, Initialize Upgrades");
        }
    }
}