using System.Collections.Generic;
using OverBang.GameName.Gameplay.Core.Structs;

namespace OverBang.GameName.Gameplay.Core.Interfaces
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