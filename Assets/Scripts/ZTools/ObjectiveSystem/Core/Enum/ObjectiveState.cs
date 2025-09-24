namespace ZTools.ObjectiveSystem.Core.Enum
{
    /// <summary>
    /// Defines the possible states an objective can be in.
    /// </summary>
    public enum ObjectiveState
    {
        Inactive,   // Objective not yet started or relevant
        Active,     // Objective is currently being tracked
        
        Completed,
        Failed,
        Disposed
    }
}