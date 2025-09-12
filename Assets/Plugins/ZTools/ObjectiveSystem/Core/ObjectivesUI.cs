using UnityEngine;
using ZTools.ObjectiveSystem.Core.Interfaces;

namespace ZTools.ObjectiveSystem.Core
{
    /// <summary>
    /// Base class for UI components that display objectives.
    /// </summary>
    public abstract class ObjectivesUI : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            ObjectivesManager.OnObjectiveChanged += OnObjectiveChanged;
            ObjectivesManager.OnObjectiveProgress += UpdateObjectiveUI;
        }
        
        protected virtual void OnDisable()
        {
            ObjectivesManager.OnObjectiveChanged -= OnObjectiveChanged;
            ObjectivesManager.OnObjectiveProgress -= UpdateObjectiveUI;
        }

        /// <summary>
        /// Called when the current objective changes.
        /// </summary>
        /// <param name="objectiveHandler">The new objective</param>
        protected abstract void OnObjectiveChanged(IObjectiveHandler objectiveHandler);

        /// <summary>
        /// Updates the UI to reflect the current state of the objective.
        /// Called when an objective is changed or its progress is updated.
        /// </summary>
        /// <param name="objectiveHandler">The <see cref="IObjectiveHandler"/> that made progress.</param>
        protected abstract void UpdateObjectiveUI(IObjectiveHandler objectiveHandler);

        /// <summary>
        /// Happens when there is no more objectives to display.
        /// Called when the current objective is cleared or completed.
        /// </summary>
        protected abstract void ClearObjective();
    }
}