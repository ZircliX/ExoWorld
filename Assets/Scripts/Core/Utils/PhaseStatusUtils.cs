namespace OverBang.GameName.Core
{
    public static class PhaseStatusUtils
    {
        public static PhaseStatus UpTo(PhaseStatus targetPhase)
        {
            PhaseStatus result = PhaseStatus.None;
    
            PhaseStatus[] phases = new[]
            {
                PhaseStatus.SceneLoaded,
                PhaseStatus.LevelLoaded,
                PhaseStatus.PlayerSetup,
                PhaseStatus.EnemiesSetup,
                PhaseStatus.UISetup,
                PhaseStatus.PoolsLoaded
            };
    
            foreach (PhaseStatus phase in phases)
            {
                result |= phase;
                if (phase == targetPhase) break;
            }
    
            return result;
        }
    }
}