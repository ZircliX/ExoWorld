using System;
using System.Collections.Generic;
using ZTools.Logger.Core;
using ZTools.Logger.Core.Enums;
using ZTools.ObjectiveSystem.Core.Data;
using ZTools.ObjectiveSystem.Core.Enum;
using ZTools.ObjectiveSystem.Core.Interfaces;

namespace ZTools.ObjectiveSystem.Core
{
    /// <summary>
    /// Manages all active objectives. Dispatches game gameEvents and orchestrates objective lifecycle.
    /// </summary>
    public static class ObjectivesManager
    {
        /// <summary>
        /// A list of currently active objectives.
        /// </summary>
        public static List<IObjectiveHandler> ActiveObjectives { get; private set; }

        // Events to notify systems about objective changes
        public static event Action<IObjectiveHandler> OnWantsToChangeObjective; // Fired just between an objective being completed and the next one being set
        public static event Action<IObjectiveHandler> OnObjectiveChanged;
        public static event Action<IObjectiveHandler> OnObjectiveProgress;
        public static event Action<IGameEvent> OnGameEventDispatched;

        public static readonly LogProvider LogProvider;
        
        /// <summary>
        /// Static constructor for ObjectivesManager.
        /// This is executed automatically the first time any member of ObjectivesManager is accessed.
        /// </summary>
        static ObjectivesManager()
        {
            ActiveObjectives = new List<IObjectiveHandler>();
            LogProvider = new LogProvider("Objectives Manager", "ObjectiveSystem");
        }
        
        /// <summary>
        /// Adds a new objective to the system, initializing it.
        /// </summary>
        /// <param name="objectiveData">The ScriptableObject defining the objective to add.</param>
        public static void AddObjective(this ObjectiveData objectiveData)
        {
            if (objectiveData == null)
            {
                LogProvider.LogError("Attempted to add a null objective data.");
                return;
            }
            
            // Use the Factory to create the runtime IObjective instance
            IObjectiveHandler newObjectiveHandler = objectiveData.GetHandler();

            if (newObjectiveHandler != null)
            {
                if (!newObjectiveHandler.InitializeObjective(objectiveData)) return;
                
                LogProvider.Log($"Objective '{objectiveData.ObjectiveName}' initialized successfully.");
                ActiveObjectives.Add(newObjectiveHandler);

                // Subscribe to the objective's progress and state change events
                newObjectiveHandler.OnObjectiveStateChanged += HandleObjectiveStateChanged;

                // If this is the new "current" objective, notify listeners
                if (ActiveObjectives[^1] == newObjectiveHandler)
                {
                    OnObjectiveChanged?.Invoke(newObjectiveHandler);
                }
                LogProvider.Log($"Objective '{objectiveData.ObjectiveName}' added and initialized.");
            }
            else
            {
                LogProvider.LogError($"Failed to create an objective handler for '{objectiveData.ObjectiveName}'. Ensure the GetHandler method is implemented correctly.");
            }
        }
        
        /// <summary>
        /// This is typically called by the manager when an objective notifies its completion.
        /// </summary>
        /// <param name="objectiveHandler">The objective to complete and dispose.</param>
        private static void CompleteAndDisposeObjective(IObjectiveHandler objectiveHandler)
        {
            if (ActiveObjectives.Contains(objectiveHandler))
            {
                //TODO : Rework for proper objective chain management
                //Unsubscribe from its events before disposing
                objectiveHandler.OnObjectiveStateChanged -= HandleObjectiveStateChanged;

                objectiveHandler.DisposeObjective();
                ActiveObjectives.Remove(objectiveHandler);
                OnWantsToChangeObjective?.Invoke(objectiveHandler);

                // Notify about the new current objective (or none)
                if (TryGetCurrentObjective(out IObjectiveHandler newObjective))
                {
                    OnObjectiveChanged?.Invoke(newObjective);
                }
                else
                {
                    OnObjectiveChanged?.Invoke(default); // No active objective
                }
            }
            else
            {
                LogProvider.LogWarning($"Objective '{objectiveHandler.ObjectiveData.ObjectiveName}' not found in the active objectives list for completion.");
            }
        }

        public static void RemoveObjective(ObjectiveData objectiveData)
        {
            RemoveObjective(objectiveData.GetHandler());
        }

        /// <summary>
        /// Removes an objective from the system without marking it as completed.
        /// Used for objectives that might be canceled or become irrelevant.
        /// </summary>
        /// <param name="objectiveHandler">The objective to remove.</param>
        public static void RemoveObjective(IObjectiveHandler objectiveHandler)
        {
            if (ActiveObjectives.Contains(objectiveHandler))
            {
                // Unsubscribe from its events before disposing
                objectiveHandler.OnObjectiveStateChanged -= HandleObjectiveStateChanged;

                OnWantsToChangeObjective?.Invoke(objectiveHandler);
                objectiveHandler.DisposeObjective(); // Still dispose to clean up
                ActiveObjectives.Remove(objectiveHandler);
                LogProvider.Log($"Objective '{objectiveHandler.ObjectiveData.ObjectiveName}' removed.");

                // Notify about the new current objective (or none)
                if (TryGetCurrentObjective(out IObjectiveHandler newObjective))
                {
                    OnObjectiveChanged?.Invoke(newObjective);
                }
                else
                {
                    OnObjectiveChanged?.Invoke(default);
                }
            }
            else
            {
                LogProvider.LogWarning($"Objective '{objectiveHandler.ObjectiveData.ObjectiveName}' not found in the active objectives list.");
            }
        }

        /// <summary>
        /// Dispatches a game gameEvent to all active objectives.
        /// This is the central point for game events to notify objectives.
        /// </summary>
        /// <param name="gameEvent">The game gameEvent to dispatch.</param>
        public static void DispatchGameEvent(IGameEvent gameEvent)
        {
            OnGameEventDispatched?.Invoke(gameEvent);
            
            for (int index = ActiveObjectives.Count - 1; index >= 0; index--)
            {
                IObjectiveHandler objectiveHandler = ActiveObjectives[index];
                
                // Only process gameEvent if the objective is in an Active state
                if (objectiveHandler.State == ObjectiveState.Active)
                {
                    bool madeProgress = objectiveHandler.ProcessEvent(gameEvent);
                    if (madeProgress) OnObjectiveProgress?.Invoke(objectiveHandler);
                }
            }
        }

        /// <summary>
        /// Tries to get the current top objective in the list.
        /// </summary>
        /// <param name="objectiveHandler">The current objective, if found.</param>
        /// <returns>True if an objective is found, false otherwise.</returns>
        public static bool TryGetCurrentObjective(out IObjectiveHandler objectiveHandler)
        {
            objectiveHandler = default;
            
            if (ActiveObjectives.Count > 0)
            {
                // Accessing the last element as the "current" objective (stack-like)
                objectiveHandler = ActiveObjectives[^1];
                return true;
            }

            return false;
        }

        // --- Event Handlers ---
        private static void HandleObjectiveStateChanged(IObjectiveHandler objectiveHandler, ObjectiveState newState)
        {
            LogProvider.Log($"Objective '{objectiveHandler.ObjectiveData.ObjectiveName}' state changed to: {newState}");
            if (newState == ObjectiveState.Completed)
            {
                CompleteAndDisposeObjective(objectiveHandler);
            }
        }
    }
}