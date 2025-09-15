namespace ZTools.ObjectiveSystem.Core.Interfaces
{
    /// <summary>
    /// Marker interface for all game events objects.
    /// Encapsulates specific data used to calculate an objective's progression.
    /// </summary>
    public interface IGameEvent
    {
        // Events often don't have methods themselves, they are just data containers.
        // Their type (e.g., KillEnemyEvent from Sample) identifies the action.
    }
}