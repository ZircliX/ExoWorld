using System;
using UnityEngine;
using ZTools.Logger.Core.Interfaces;
using ZTools.ObjectiveSystem.Core.Data;
using ZTools.ObjectiveSystem.Core.Enum;
using ZTools.ObjectiveSystem.Core.Interfaces;

namespace ZTools.ObjectiveSystem.Core
{
    public abstract class ObjectiveHandler<TS, TV> : IObjectiveHandler, ILogSource
        where TS : ObjectiveData
        where TV : IGameEvent, new()
    {
        /// <summary>
        /// Required Name property for the <see cref="ILogSource"/> interface.
        /// </summary>
        public string Name => ObjectiveData.Name;

        /// <summary>
        /// The typed data associated with this objective handler.
        /// Only used internally to avoid casting in derived classes.
        /// </summary>
        private TS DataTyped;
        
        /// <summary>
        /// Gets the base <see cref="ObjectiveData"/> associated with this objective.
        /// </summary>
        public ObjectiveData ObjectiveData { get; protected set; }
        public ObjectiveData GetObjectiveData() => ObjectiveData;

        /// <summary>
        /// Gets a value indicating whether the objective has met its completion criteria.
        /// </summary>
        protected bool IsCompleted => CurrentProgress.currentProgress >= CurrentProgress.targetProgress;
        protected bool GetCompleted() => IsCompleted;

        /// <summary>
        /// This value typically increments as the player performs actions related to the objective.
        /// </summary>
        public ObjectiveProgression CurrentProgress { get; protected set; }
        public ObjectiveProgression GetCurrentProgress() => CurrentProgress;

        /// <summary>
        /// Indicates where the objective is in its lifecycle.
        /// </summary>
        public ObjectiveState State { get; protected set; }
        public ObjectiveState GetState() => State;

        /// <summary>
        /// Event fired when the objective's progress changes.
        /// Subscribers can react to updates in the objective's completion status.
        /// </summary>
        public event Action<IObjectiveHandler> OnObjectiveProgressChanged = delegate { };

        /// <summary>
        /// Invokes the <see cref="OnObjectiveProgressChanged"/> event.
        /// </summary>
        protected void InvokeProgressChanged()
            => OnObjectiveProgressChanged?.Invoke(this);

        /// <summary>
        /// Event fired when the objective's state changes (e.g., from Active to Completed).
        /// Subscribers can react to significant lifecycle events of the objective.
        /// </summary>
        public event Action<IObjectiveHandler, ObjectiveState> OnObjectiveStateChanged = delegate { };

        /// <summary>
        /// Invokes the <see cref="OnObjectiveStateChanged"/> event with the new state.
        /// </summary>
        /// <param name="newState">The new <see cref="ObjectiveState"/> of the objective.</param>
        protected void InvokeStateChanged(ObjectiveState newState)
            => OnObjectiveStateChanged?.Invoke(this, newState);

        /// <summary>
        /// Initializes the objective and sets it to its active state, preparing it for tracking.
        /// </summary>
        /// <param name="data">The ScriptableObject containing the objective's static definition.</param>
        public virtual bool InitializeObjective(ObjectiveData data)
        {
            if (data is TS DataTS)
            {
                DataTyped = DataTS;
                ObjectiveData = DataTS;

                CurrentProgress = default;
                SetState(ObjectiveState.Active);
                
                return true;
            }
            
            ObjectivesManager.LogProvider.LogError(this, $"ObjectiveHandler initialized with incorrect data type : {data.GetType().Name}. Expected type: " + typeof(TS).Name);
            return false;
        }

        /// <summary>
        /// Disposes of the objective, setting its state to Disposed and performing any necessary cleanup.
        /// This method should be called when an objective is no longer active (e.g., completed, failed, or canceled).
        /// </summary>
        public virtual void DisposeObjective()
        {
            SetState(ObjectiveState.Disposed);
            ObjectivesManager.LogProvider.Log(this, $"Disposed objective : {ObjectiveData.ObjectiveName}");
        }

        /// <summary>
        /// Processes a game gameEvent relevant to this objective's progress.
        /// </summary>
        /// <param name="gameEvent">The gameEvent to process.</param>
        public virtual bool ProcessEvent(IGameEvent gameEvent)
        {
            ObjectiveProgression eventProgress = CalculateProgression(gameEvent);
            
            // Check if the eventProgress is different from the current progress
            bool isDifferent = !Mathf.Approximately(eventProgress.currentProgress, CurrentProgress.currentProgress) ||
                                    !Mathf.Approximately(eventProgress.targetProgress, CurrentProgress.targetProgress);
            
            CurrentProgress = eventProgress;
            
            ObjectivesManager.LogProvider.Log(this,
                $"Objective '{ObjectiveData.ObjectiveName}' progress: {CurrentProgress.currentProgress}/{CurrentProgress.targetProgress}");

            if (IsCompleted)
            {
                SetState(ObjectiveState.Completed); // Set state, manager will handle disposal from event
                ObjectiveCompleted();
            }
            else
            {
                InvokeProgressChanged();
            }

            return isDifferent;
        }

        /// <summary>
        /// Casts the provided gameEvent to the specific type and calculates how much the objective has progressed.
        /// </summary>
        /// <param name="gameEvent">The gameEvent which is provided by the manager, you have to cast it to your objective gameEvent</param>
        /// <returns>How much the objective have progressed</returns>
        protected virtual ObjectiveProgression CalculateProgression(IGameEvent gameEvent)
        {
            if (gameEvent is TV gameEventTV)
            {
                return CalculateProgression(DataTyped, gameEventTV);
            }

            return CurrentProgress;
        }

        /// <summary>
        /// Abstract method to calculate the progression of the objective based on the provided data and gameEvent.
        /// </summary>
        /// <param name="objectiveData">The typed objectiveData given to avoid casting.</param>
        /// <param name="gameEvent">the typed gameEvent given to avoid casting</param>
        /// <returns>How much the objective have progressed</returns>
        protected abstract ObjectiveProgression CalculateProgression(TS objectiveData, TV gameEvent);

        /// <summary>
        /// Called when the objective is completed.
        /// Can be used to reward the player, trigger events, or perform other actions.
        /// </summary>
        protected abstract void ObjectiveCompleted();

        /// <summary>
        /// Protected method to set the objective's state and notify listeners.
        /// </summary>
        protected virtual void SetState(ObjectiveState newState)
        {
            if (State == newState) return;
            State = newState;
            InvokeStateChanged(newState);
        }
    }
}