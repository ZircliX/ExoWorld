using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class PlayerEntity : MonoBehaviour, IPlayerComponent
    {
        
        [field: SerializeField] public DamageableAndHealableComponent DamageableAndHealableComponent { get; private set; }
        public PlayerController Controller { get; set; }
        private CharacterData characterData { get; set; }
        
        public void OnSync(PlayerRuntimeContext context)
        {
            characterData = context.playerCharacterData;
            Initialize();
        }
        
        
        private void Initialize()
        {
            DamageableAndHealableComponent.Initialize(
                characterData.CharacterBaseStats.Health + UpgradeManager.Instance.GetRuntimeUpgrade(UpgradeType.Health),
                characterData.CharacterBaseStats.Resistance + UpgradeManager.Instance.GetRuntimeUpgrade(UpgradeType.Resistance));
        }
    }
}