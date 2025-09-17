using TMPro;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Health
{
    public class HealthComponentWithUI : HealthComponent
    {
        [field: SerializeField] public TMP_Text healthText { get; private set; }

        private void OnEnable()
        {
            OnHealthChanged += RefreshHealthText;
        }
        
        private void OnDisable()
        {
            OnHealthChanged -= RefreshHealthText;
        }

        private void Start()
        {
            RefreshHealthText(Mathf.RoundToInt(Health));
        }

        protected void RefreshHealthText(int _health)
        {
            healthText.text = $"{_health} / {MaxHealthPriority.Value}";
        }
    }
}