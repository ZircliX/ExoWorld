using System.Collections;

namespace Health.Core
{
    public abstract class EffectCommand
    {
        public abstract float CurrentValue { get; protected set; }
        
        public abstract IEnumerator Execute(IEffectReceiver receiver, EffectData effectData);
    }
}