namespace OverBang.Pooling.Resource
{
    public interface IPoolInstanceListener
    {
        IPool Pool { get; }
        
        void OnSpawn(IPool pool);
        void OnDespawn(IPool pool);
    }
}