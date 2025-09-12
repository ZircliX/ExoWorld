using UnityEngine;

namespace ZTools.ObjectiveSystem.Core
{
    [System.Serializable]
    public struct ObjectiveProgression
    {
        public enum ProgressionDisplay
        {
            /// <summary>
            /// Display the current progress as a fraction of the target.
            /// </summary>
            Fraction,
            
            /// <summary>
            /// Display the current progress as a percentage of the target.
            /// </summary>
            Percentage,
            
            /// <summary>
            /// Display the current progress as a checkbox.
            /// </summary>
            CheckBox
        }
        
        public float currentProgress;
        public float targetProgress;
        
        public ProgressionDisplay DisplayMode;
        
        public ObjectiveProgression(float currentProgress, float targetProgress)
        {
            this.currentProgress = currentProgress;
            this.targetProgress = targetProgress;
            DisplayMode = ProgressionDisplay.Fraction;
        }
        
        public override string ToString()
        {
            return DisplayMode switch
            {
                ProgressionDisplay.Fraction => $"{currentProgress}/{targetProgress}",
                ProgressionDisplay.Percentage => $"{(currentProgress / targetProgress) * 100f:0.00}%",
                ProgressionDisplay.CheckBox => currentProgress >= targetProgress ? "✔" : "✘", //TODO: Chelou
                _ => $"{currentProgress}/{targetProgress}"
            };
        }
    }
}