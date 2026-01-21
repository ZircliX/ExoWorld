using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    public class PlayerEntity : MonoBehaviour, IPlayerComponent
    {
        
        [field: SerializeField] public DamageableAndHealableComponent DamageableAndHealableComponent { get; private set; }
        public PlayerController Controller { get; set; }
        private CharacterData characterData { get; set; }

        private void OnEnable()
        {
            UpgradeManager.Instance.OnUpgrade += Initialize;
        }

        private void OnDisable()
        {
            UpgradeManager.Instance.OnUpgrade -= Initialize;
            
        }

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