using Health.Core;
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
                EffectData effectData = new EffectData.Builder()
                                        .SetAmount(30)
                                        .SetDuration(10)
                                        .SetSteps(3)
                                        .Build();
                
                healthComponent.ApplyEffect(effectData);
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                EffectData effectData = new EffectData.Builder()
                                        .SetAmount(-5)
                                        .SetDuration(5)
                                        .Build();
                
                healthComponent.ApplyEffect(effectData);
            }
        }
    }
}