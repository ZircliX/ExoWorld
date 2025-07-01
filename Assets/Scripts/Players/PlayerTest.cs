using Health.Core;
using Health.Implementations;
using Health.Structs;
using UnityEngine;

namespace Health.Players
{
    public class PlayerTest : MonoBehaviour
    {
        [SerializeField] private HealthComponent healthComponent;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                HealingData healingData = new HealingData(10f, 1f);
                healthComponent.Heal(HealingMetrics.HealthPercentage, healingData);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                healthComponent.Damage(50);
            }
        }
    }
}