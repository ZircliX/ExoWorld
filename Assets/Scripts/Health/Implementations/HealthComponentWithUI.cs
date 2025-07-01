using Health.Core;
using TMPro;
using UnityEngine;

namespace Health.Implementations
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

        protected void RefreshHealthText(float _health, float _healthBar)
        {
            healthText.text = $"{Mathf.FloorToInt(health)} / {MaxHealth.Value}";
        }
    }
}