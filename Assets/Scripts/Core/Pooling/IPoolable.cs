namespace OverBang.GameName.Core.Pooling
{
    public interface IPoolable
    {
        string PoolName { get; }
        
        void OnSpawn();
        void OnDespawn();
    }
}