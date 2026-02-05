using System.Collections.Generic;
using OverBang.ExoWorld.Gameplay.Level;
using UnityEngine;
using ZTools.Logger.Core;
using ZTools.ObjectiveSystem.Core;
using ZTools.RewardSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    /// <summary>
    /// Manages the sequential queuing of objectives from predefined collections.
    /// This script acts as a provider that listens for objective completion events
    /// and automatically adds the next objective in the sequence.
    /// </summary>
    public class ObjectiveProvider : MonoBehaviour, ILogSource
    {
        /// <summary>
        /// Required Name property for the <see cref="ILogSource"/> interface.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// A list of <see cref="ObjectiveCollectionData"/> assets, each containing
        /// a sequence of objectives to be provided in order.
        /// </summary>
        [SerializeField] private List<ObjectiveCollectionData> objectiveCollections;

        /// <summary>
        /// The current index of the <see cref="ObjectiveCollectionData"/> being processed.
        /// </summary>
        private int currentCollectionIndex;

        /// <summary>
        /// The current index of the <see cref="ObjectiveData"/> within the current
        /// <see cref="ObjectiveCollectionData"/> being processed.
        /// </summary>
        private int currentObjectiveIndex;
        
        private void OnEnable()
        {
            if (LevelManager.Instance != null)
                LevelManager.Instance.OnStateChanged += OnStateChanged;
            //ObjectivesManager.OnWantsToChangeObjective += QueueObjective;
        }
        
        private void OnDisable()
        {
            if (LevelManager.Instance != null)
                LevelManager.Instance.OnStateChanged -= OnStateChanged;
            //ObjectivesManager.OnWantsToChangeObjective -= QueueObjective;
        }

        private void OnStateChanged(LevelState state)
        {
            if (state == LevelState.Running)
            {
                ObjectiveCollectionData obj = objectiveCollections[0];
                for (int index = obj.Objectives.Length - 1; index >= 0; index--)
                {
                    ObjectiveData objData = obj.Objectives[index];
                    objData.AddObjective();
                }

                //QueueObjective();
            }
        }

        /*
        /// <summary>
        /// This is a temporary method to kick off the queuing process.
        /// </summary>
        private void Start()
        {
            QueueObjective();
        }*/

        /// <summary>
        /// Advances to the next <see cref="ObjectiveCollectionData"/> in the sequence.
        /// If all collections have been processed, it unsubscribes from further objective
        /// change events, effectively stopping the queuing process.
        /// </summary>
        private void ChangeCollection()
        {
            RewardManager.ProcessRewards(objectiveCollections[currentCollectionIndex].Rewards);
            
            currentCollectionIndex++;
            currentObjectiveIndex = 0; // Reset objective index for the new collection
            
            if (currentCollectionIndex >= objectiveCollections.Count)
            {
                // If all collections are processed, stop queuing new objectives
                ObjectivesManager.OnWantsToChangeObjective -= QueueObjective;
                ObjectivesManager.LogProvider.Log(this, "All objective collections completed. ObjectiveProvider is no longer queuing objectives.");
            }
        }
        
        /// <summary>
        /// Queues the next objective from the current collection to the <see cref="ObjectivesManager"/>.
        /// </summary>
        /// <param name="objectiveHandler">
        /// This parameter is ignored if simply queueing the next objective.
        /// </param>
        public void QueueObjective(IObjectiveHandler objectiveHandler = null)
        {
            ObjectiveCollectionData currentCollection = objectiveCollections[currentCollectionIndex];
            if (currentCollection == null || currentCollection.Objectives == null)
            {
                ObjectivesManager.LogProvider.LogWarning(this, $"ObjectiveCollection at index {currentCollectionIndex} is empty or null.");
                ChangeCollection(); // Try next collection
                return;
            }

            if (currentObjectiveIndex >= currentCollection.Objectives.Length)
            {
                // Current collection exhausted, move to the next one
                ChangeCollection();
                ObjectivesManager.LogProvider.Log("ObjectiveIndex = " + currentObjectiveIndex + " / " + currentCollection.Objectives.Length);
                ObjectivesManager.LogProvider.Log("ObjectiveCollectionIndex = " + currentCollectionIndex + " / " + objectiveCollections.Count);
                // After changing collection, try to queue from the new one if available
                if (currentCollectionIndex < objectiveCollections.Count)
                {
                    QueueObjective(); 
                }
                return;
            }

            // Get the next objective data
            ObjectiveData objectiveData = currentCollection.Objectives[currentObjectiveIndex];
            ObjectivesManager.LogProvider.Log(this, $"Queued objective: '{objectiveData.ObjectiveName}' from collection '{currentCollection.name}'.");
            objectiveData.AddObjective();
            
            currentObjectiveIndex++;
            // Check if the current collection is now exhausted after queuing this objective
            if (currentObjectiveIndex > currentCollection.Objectives.Length)
            {
                ObjectivesManager.LogProvider.Log("ObjectiveIndex = " + currentObjectiveIndex + " / " + currentCollection.Objectives.Length);
                ObjectivesManager.LogProvider.Log("ObjectiveCollectionIndex = " + currentCollectionIndex + " / " + objectiveCollections.Count);
                ObjectivesManager.LogProvider.Log(this, $"Collection '{currentCollection.name}' exhausted. Moving to next collection.");
                ChangeCollection();
            }
        }
    }
}