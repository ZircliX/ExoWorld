using System;
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

        private void Start()
        {
            RefreshHealthText(Mathf.RoundToInt(health));
        }

        protected void RefreshHealthText(int _health)
        {
            //Debug.Log(health);
            healthText.text = $"{_health} / {MaxHealthPriority.Value}";
        }
    }
}