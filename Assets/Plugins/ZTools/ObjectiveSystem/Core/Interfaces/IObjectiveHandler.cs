using System;
using ZTools.ObjectiveSystem.Core.Data;
using ZTools.ObjectiveSystem.Core.Enum;

namespace ZTools.ObjectiveSystem.Core.Interfaces
{
    /// <summary>
    /// The non-generic base interface for all runtime objectives.
    /// This allows the ObjectivesManager to treat all objectives polymorphically.
    /// </summary>
    public interface IObjectiveHandler
    {
        /// <summary>
        /// Gets the base objective data (polymorphic access to common properties).
        /// </summary>
        ObjectiveData ObjectiveData { get; }

        /// <summary>
        /// Gets the current progress towards objective completion.
        /// </summary>
        ObjectiveProgression CurrentProgress { get; }

        /// <summary>
        /// The current state of the objective (<see cref="ObjectiveState"/>).
        /// </summary>
        ObjectiveState State { get; }
        
        event Action<IObjectiveHandler> OnObjectiveProgressChanged;
        
        event Action<IObjectiveHandler, ObjectiveState> OnObjectiveStateChanged;

        /// <summary>
        /// Initializes the objective and sets it to its initial state.
        /// </summary>
        bool InitializeObjective(ObjectiveData data);

        /// <summary>
        /// Disposes of the objective, cleans up subscriptions, and sets it to a disposed state.
        /// </summary>
        void DisposeObjective();

        /// <summary>
        /// Processes a game gameEvent, potentially updating the objective's progress.
        /// </summary>
        /// <param name="gameEvent">The game gameEvent to process.</param>
        bool ProcessEvent(IGameEvent gameEvent);
    }
}