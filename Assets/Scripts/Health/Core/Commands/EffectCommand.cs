using System.Collections;

namespace Health.Core
{
    public abstract class EffectCommand : IEffectCommand
    {
        public abstract IEnumerator Execute(IEffectContext context, EffectData effectData);
    }
}