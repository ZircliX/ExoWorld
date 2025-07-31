using Health.Core;
using UnityEngine;

namespace Health.Players
{
    public class HealthPlayerSample : MonoBehaviour
    {
        [SerializeField] private HealthComponent healthComponent;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                EffectData effectData = new EffectData.Builder()
                                        .SetAmount(30)
                                        .SetDuration(3)
                                        .SetSteps(30)
                                        .Build();
                
                healthComponent.RegisterEffectCommand(effectData);
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                EffectData effectData = new EffectData.Builder()
                                        .SetAmount(-5)
                                        .SetDuration(1)
                                        .Build();
                
                healthComponent.RegisterEffectCommand(effectData);
            }
        }
    }
}