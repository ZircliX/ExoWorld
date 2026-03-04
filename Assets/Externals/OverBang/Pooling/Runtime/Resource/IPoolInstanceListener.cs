namespace OverBang.Pooling.Resource
{
    public interface IPoolInstanceListener
    {
        void OnSpawn(IPool pool);
        void OnDespawn(IPool pool);
    }
}