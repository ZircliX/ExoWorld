using System.Collections;
using Health.Structs;

namespace Health.Core
{
    public abstract class HealingCommand
    {
        public abstract IEnumerator Execute(HealthComponent healthComponent, HealingData healingData,
            HealingCommandArgs healingCommandArgs);
        
    }
}