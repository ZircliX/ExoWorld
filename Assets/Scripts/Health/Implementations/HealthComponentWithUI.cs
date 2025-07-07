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
            RefreshHealthText(health);
        }

        protected void RefreshHealthText(float _health)
        {
            //Debug.Log(health);
            healthText.text = $"{Mathf.CeilToInt(health)} / {MaxHealthPriority.Value}";
        }
    }
}