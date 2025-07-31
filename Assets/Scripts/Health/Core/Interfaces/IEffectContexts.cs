using System.Collections.Generic;

namespace Health.Core
{
    public interface IEffectReceiver
    {
        List<EffectCommand> EffectCommands { get; }
        
        void RegisterEffectCommand(EffectData effectData);
        void UnregisterEffectCommand(EffectCommand command);
        
        void OnEffectTick(EffectCommand command);
        
        float MaxValue { get; }
    }
}