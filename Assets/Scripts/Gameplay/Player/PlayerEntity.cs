using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    public class PlayerEntity : MonoBehaviour, IPlayerComponent, ITargetable
    {
        [field: SerializeField] public DamageableAndHealableComponent DamageableAndHealableComponent { get; private set; }
        public PlayerController Controller { get; set; }
        private CharacterData characterData;

        private void OnEnable() => UpgradeManager.Instance.OnUpgrade += Initialize;

        private void OnDisable() => UpgradeManager.Instance.OnUpgrade -= Initialize;

        public void OnSync(PlayerRuntimeContext context)
        {
            characterData = context.playerCharacterData;
            Initialize();
        }
        
        private void Initialize()
        {
            DamageableAndHealableComponent.Initialize(
                characterData.BaseStats.Health + UpgradeManager.Instance.GetRuntimeUpgrade(UpgradeType.Health),
                characterData.BaseStats.Resistance + UpgradeManager.Instance.GetRuntimeUpgrade(UpgradeType.Resistance));
        }

        public event Action<bool> OnTargetableChanged;
        public Transform Transform => transform;
        public TargetPriority Priority => TargetPriority.High;
        public bool IsTargetable { get; private set; }
        public void SetTargetable(bool state)
        {
            IsTargetable = state;
            OnTargetableChanged?.Invoke(IsTargetable);
        }
    }
}