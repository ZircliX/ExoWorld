using System;
using OverBang.GameName.Core.GameMode;

namespace OverBang.GameName.Gameplay.Gameplay.StateMachine
{
    public class StateMachine<T> where T : IGameState
    {
        public T CurrentState { get; private set; }
        public event Action<T> OnStateChanged;

        public void ChangeState(T newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
            OnStateChanged?.Invoke(CurrentState);
        }
    }
}